using System.Data.Common;

namespace Core.Application.Abstractions.Database;

public interface ISqlScriptTracker
{
	Task EnsureCreatedAsync(DbConnection connection, CancellationToken ct);
	Task<HashSet<string>> GetAppliedAsync(DbConnection connection, CancellationToken ct);
	Task MarkAppliedAsync(DbConnection connection, string scriptName, CancellationToken ct);
}
