using System.Reflection;

namespace Building.Domain;

public static class AssemblyReference
{
	public const string Instance = "buildings";

	public const string MigrationHistoryTableName = "migration_history";

	public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
