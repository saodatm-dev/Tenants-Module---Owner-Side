using System.Reflection;
using System.Text;
using Core.Application.Abstractions.Database;
using Microsoft.Extensions.Logging;

namespace Core.Infrastructure.Database;

/// <summary>
/// Loads SQL scripts from the file system
/// </summary>
public sealed class FileSystemSqlScriptSource(Assembly assembly, string scriptsFolder, ILogger? logger) : ISqlScriptSource
{
	public Task<IReadOnlyCollection<SqlScript>> GetScriptsAsync(CancellationToken cancellationToken)
	{
		return Task.FromResult<IReadOnlyCollection<SqlScript>>(LoadScripts());
	}

	private List<SqlScript> LoadScripts()
	{
		var scripts = new List<SqlScript>();

		var assemblyLocation = assembly.Location;
		if (string.IsNullOrEmpty(assemblyLocation))
		{
			logger?.LogWarning("Could not determine assembly location");
			return scripts;
		}

		var assemblyDirectory = Path.GetDirectoryName(assemblyLocation);
		if (string.IsNullOrEmpty(assemblyDirectory))
		{
			logger?.LogWarning("Could not determine assembly directory");
			return scripts;
		}

		var scriptsPath = Path.Combine(assemblyDirectory, scriptsFolder);

		if (!Directory.Exists(scriptsPath))
		{
			var currentDir = Directory.GetCurrentDirectory();
			var possiblePaths = new[]
			{
				Path.Combine(currentDir, scriptsFolder),
				Path.Combine(assemblyDirectory, "..", "..", "..", "..", scriptsFolder),
				Path.Combine(assemblyDirectory, "..", scriptsFolder),
				Path.Combine(assemblyDirectory, "..", "..", scriptsFolder)
			};

			foreach (var path in possiblePaths)
			{
				var normalizedPath = Path.GetFullPath(path);
				if (Directory.Exists(normalizedPath))
				{
					scriptsPath = normalizedPath;
					break;
				}
			}
		}

		if (!Directory.Exists(scriptsPath))
		{
			logger?.LogWarning("DatabaseProgramming folder not found at: {Path}", scriptsPath);
			return scripts;
		}

		logger?.LogDebug("Loading SQL scripts from: {Path}", scriptsPath);

		var sqlFiles = Directory.GetFiles(scriptsPath, "*.sql", SearchOption.AllDirectories)
			.OrderBy(f => f)
			.ToList();

		foreach (var filePath in sqlFiles)
		{
			try
			{
				var content = File.ReadAllText(filePath, Encoding.UTF8);
				var fileName = Path.GetFileName(filePath);
				var relativePath = Path.GetRelativePath(scriptsPath, filePath);

				scripts.Add(new SqlScript
				{
					FileName = fileName,
					FilePath = relativePath,
					Content = content,
					ResourceName = filePath
				});
			}
			catch (Exception ex)
			{
				logger?.LogError(ex, "Failed to load SQL script from file: {FilePath}", filePath);
			}
		}

		return scripts;
	}
}
