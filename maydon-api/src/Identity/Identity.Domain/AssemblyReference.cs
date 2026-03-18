using System.Reflection;

namespace Identity.Domain;

public static class AssemblyReference
{
	public const string Instance = "identity";

	public const string MigrationHistoryTableName = "migration_history";

	public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
