using System.Data.Common;
using Core.Application.Abstractions.Database;
using Npgsql;

namespace Core.Infrastructure.Database;

/// <summary>
/// Tracks applied SQL scripts in PostgreSQL database
/// </summary>
public sealed class PostgresSqlScriptTracker(DatabaseScriptExecutorOptions options) : ISqlScriptTracker
{
	public async Task EnsureCreatedAsync(DbConnection connection, CancellationToken ct)
	{
		var cmd = connection.CreateCommand();
		cmd.CommandText = $"""
                           CREATE TABLE IF NOT EXISTS {options.TrackingSchema}.{options.TrackingTable} (
                               script_name VARCHAR(500) PRIMARY KEY,
                               applied_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
                               applied_by VARCHAR(255),
                               script_content_hash VARCHAR(64)
                           );
                           """;

		await cmd.ExecuteNonQueryAsync(ct);
	}

	public async Task<HashSet<string>> GetAppliedAsync(DbConnection connection, CancellationToken ct)
	{
		var cmd = connection.CreateCommand();
		cmd.CommandText = $"SELECT script_name FROM {options.TrackingSchema}.{options.TrackingTable};";

		var result = new HashSet<string>();
		using var reader = await cmd.ExecuteReaderAsync(ct);

		while (await reader.ReadAsync(ct))
			result.Add(reader.GetString(0));

		return result;
	}

	public async Task MarkAppliedAsync(DbConnection connection, string scriptName, CancellationToken ct)
	{
		var cmd = connection.CreateCommand();
		cmd.CommandText = $"""
                           INSERT INTO {options.TrackingSchema}.{options.TrackingTable} (script_name, applied_at)
                           VALUES (@scriptName, NOW())
                           ON CONFLICT (script_name) DO NOTHING;
                           """;

		cmd.Parameters.Add(new NpgsqlParameter("scriptName", scriptName));
		await cmd.ExecuteNonQueryAsync(ct);
	}
}
