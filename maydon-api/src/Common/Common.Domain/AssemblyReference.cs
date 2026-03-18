using System.Reflection;

namespace Common.Domain;

public static class AssemblyReference
{
	public const string Instance = "commons";

	public const string MigrationHistoryTableName = "migration_history";

	public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
