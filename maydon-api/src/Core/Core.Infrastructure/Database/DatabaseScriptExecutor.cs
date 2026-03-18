using System.Data;
using System.Reflection;
using System.Text;
using Core.Application.Abstractions.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Core.Infrastructure.Database;

/// <summary>
/// Executes SQL scripts from the DatabaseProgramming folder
/// </summary>
public sealed class DatabaseScriptExecutor<TDbContext>
	where TDbContext : DbContext
{
	private readonly TDbContext _dbContext;
	private readonly ILogger? _logger;
	private readonly ISqlScriptSource _scriptSource;
	private readonly ISqlScriptTracker _tracker;
	private readonly DatabaseScriptExecutorOptions _options;

	public DatabaseScriptExecutor(
		TDbContext dbContext,
		ILogger? logger = null,
		DatabaseScriptExecutorOptions? options = null,
		ISqlScriptSource? scriptSource = null,
		ISqlScriptTracker? tracker = null)
	{
		_dbContext = dbContext;
		_logger = logger;
		_options = options ?? new DatabaseScriptExecutorOptions();

		_scriptSource = scriptSource ??
			new FileSystemSqlScriptSource(
				Assembly.GetAssembly(typeof(TDbContext))!,
				_options.ScriptsFolder,
				logger);

		_tracker = tracker ?? new PostgresSqlScriptTracker(_options);
	}

	public async Task ExecuteScriptsAsync(NpgsqlConnection? connection = null, CancellationToken cancellationToken = default)
	{
		connection ??= await OpenConnectionAsync(cancellationToken);

		await _tracker.EnsureCreatedAsync(connection, cancellationToken);

		var scripts = await _scriptSource.GetScriptsAsync(cancellationToken);
		if (!scripts.Any())
		{
			_logger?.LogInformation("No SQL scripts found");
			return;
		}

		_logger?.LogInformation("Found {Count} SQL script(s)", scripts.Count);

		var appliedScripts = await _tracker.GetAppliedAsync(connection, cancellationToken);

		foreach (var script in scripts.OrderBy(s => s.FilePath))
		{
			if (appliedScripts.Contains(script.FileName))
			{
				_logger?.LogDebug("Script {Script} already applied, skipping", script.FileName);
				continue;
			}

			_logger?.LogInformation("Executing script: {Script}", script.FileName);
			await ExecuteScriptAsync(connection, script, cancellationToken);
			await _tracker.MarkAppliedAsync(connection, script.FileName, cancellationToken);
			_logger?.LogInformation("Successfully executed script: {Script}", script.FileName);
		}
	}

	private async Task<NpgsqlConnection> OpenConnectionAsync(CancellationToken ct)
	{
		var connection = _dbContext.Database.GetDbConnection();

		if (connection.State != ConnectionState.Open)
			await connection.OpenAsync(ct);

		return connection as NpgsqlConnection
			?? throw new InvalidOperationException(
				$"{typeof(TDbContext).Name} must use PostgreSQL (NpgsqlConnection)");
	}

	private async Task ExecuteScriptAsync(NpgsqlConnection connection, SqlScript script, CancellationToken cancellationToken)
	{
		var containsDollarQuotes =
			script.Content.Contains("$$", StringComparison.Ordinal) ||
			ContainsTaggedDollarQuotes(script.Content);

		if (containsDollarQuotes)
		{
			await using var command = connection.CreateCommand();
			command.CommandText = script.Content.Trim();
			command.CommandTimeout = _options.CommandTimeoutSeconds;

			try
			{
				await command.ExecuteNonQueryAsync(cancellationToken);
			}
			catch (PostgresException ex)
			{
				_logger?.LogError(ex, "Failed executing script {Script}", script.FileName);
				throw;
			}
		}
		else
		{
			var statements = SplitScriptIntoStatements(script.Content);

			foreach (var statement in statements)
			{
				if (string.IsNullOrWhiteSpace(statement))
					continue;

				await using var command = connection.CreateCommand();
				command.CommandText = statement;
				command.CommandTimeout = _options.CommandTimeoutSeconds;

				try
				{
					await command.ExecuteNonQueryAsync(cancellationToken);
				}
				catch (PostgresException ex)
				{
					_logger?.LogError(
						ex,
						"Failed executing statement from {Script}. Statement: {Statement}",
						script.FileName,
						statement[..Math.Min(statement.Length, 200)]);
					throw;
				}
			}
		}
	}

	private static List<string> SplitScriptIntoStatements(string script)
	{
		var statements = new List<string>();
		var current = new StringBuilder();
		var inQuotes = false;
		var quoteChar = '\0';
		var inDollarQuotes = false;
		var dollarTag = string.Empty;

		for (int i = 0; i < script.Length; i++)
		{
			var c = script[i];

			if (!inQuotes && c == '$' && !inDollarQuotes)
			{
				var tagEnd = script.IndexOf('$', i + 1);
				if (tagEnd > i)
				{
					dollarTag = script.Substring(i, tagEnd - i + 1);
					inDollarQuotes = true;
					current.Append(c);
					continue;
				}
			}
			else if (inDollarQuotes)
			{
				current.Append(c);
				if (c == '$' &&
					script.Substring(
						Math.Max(0, i - dollarTag.Length + 1),
						Math.Min(dollarTag.Length, i + 1)) == dollarTag)
				{
					inDollarQuotes = false;
					dollarTag = string.Empty;
				}
				continue;
			}

			if ((c == '\'' || c == '"') && !inDollarQuotes)
			{
				if (!inQuotes)
				{
					inQuotes = true;
					quoteChar = c;
				}
				else if (c == quoteChar && (i == 0 || script[i - 1] != '\\'))
				{
					inQuotes = false;
					quoteChar = '\0';
				}
			}

			current.Append(c);

			if (c == ';' && !inQuotes && !inDollarQuotes)
			{
				var stmt = current.ToString().Trim();
				if (!string.IsNullOrWhiteSpace(stmt))
					statements.Add(stmt);

				current.Clear();
			}
		}

		var remaining = current.ToString().Trim();
		if (!string.IsNullOrWhiteSpace(remaining))
			statements.Add(remaining);

		return statements;
	}

	private static bool ContainsTaggedDollarQuotes(string content)
	{
		for (int i = 0; i < content.Length - 1; i++)
		{
			if (content[i] != '$') continue;

			int j = i + 1;
			if (j >= content.Length || (!char.IsLetter(content[j]) && content[j] != '_'))
				continue;

			while (j < content.Length && (char.IsLetterOrDigit(content[j]) || content[j] == '_'))
				j++;

			if (j < content.Length && content[j] == '$')
				return true;
		}

		return false;
	}
}
