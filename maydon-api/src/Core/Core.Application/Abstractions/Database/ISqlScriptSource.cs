namespace Core.Application.Abstractions.Database;

public interface ISqlScriptSource
{
	Task<IReadOnlyCollection<SqlScript>> GetScriptsAsync(CancellationToken cancellationToken);
}
