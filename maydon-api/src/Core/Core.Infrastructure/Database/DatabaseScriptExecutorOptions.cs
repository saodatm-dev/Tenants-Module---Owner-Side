namespace Core.Infrastructure.Database;

public sealed class DatabaseScriptExecutorOptions
{
	public string ScriptsFolder { get; init; } = "DatabaseProgramming";
	public string TrackingSchema { get; init; } = "public";
	public string TrackingTable { get; init; } = "__applied_sql_scripts";
	public int CommandTimeoutSeconds { get; init; } = 300;
}
