using System.Data;
using Core.Application.Abstractions.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Core.Infrastructure.Database;

/// <summary>
/// Unified database migrator that orchestrates all module migrations.
/// Creates database, ensures schemas exist, applies EF Core migrations,
/// and executes SQL scripts for each registered module.
/// </summary>
public sealed class MaydonDatabaseMigrator(ILogger<MaydonDatabaseMigrator> logger) : IDatabaseMigrator
{
    private const long AdvisoryLockId = 555_000_001;
    public const string MigrationHistoryTableName = "migration_history";

    public async Task MigrateAsync(IServiceScope scope, CancellationToken cancellationToken = default)
    {
        var registry = scope.ServiceProvider.GetRequiredService<IModuleMigrationRegistry>();
        var descriptors = registry.GetDescriptors();

        if (descriptors.Count == 0)
        {
            logger.LogWarning("No module migration descriptors registered. Skipping migration");
            return;
        }

        logger.LogInformation("Starting unified database migration for {Count} module(s)", descriptors.Count);

        // 1. Ensure database exists using the first available DbContext connection string
        var firstDbContext = ResolveDbContext(scope, descriptors[0]);
        var connectionString = firstDbContext.Database.GetConnectionString()
            ?? throw new InvalidOperationException("Connection string is not configured");

        await EnsureDatabaseExistsAsync(connectionString, cancellationToken);

        // 2. Open a shared connection for schema operations
        var dbConnection = firstDbContext.Database.GetDbConnection();

        if (dbConnection.State != ConnectionState.Open)
            await dbConnection.OpenAsync(cancellationToken);

        if (dbConnection is not NpgsqlConnection connection)
            throw new InvalidOperationException("Database connection must be a PostgreSQL connection (NpgsqlConnection)");

        // 3. Acquire advisory lock
        await AcquireAdvisoryLockAsync(connection, cancellationToken);

        try
        {
            // 4. Migrate each module in order
            foreach (var descriptor in descriptors)
            {
                await MigrateModuleAsync(scope, connection, descriptor, cancellationToken);
            }

            logger.LogInformation("Unified database migration completed successfully for all {Count} module(s)", descriptors.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred during unified database migration");
            throw;
        }
        finally
        {
            await ReleaseAdvisoryLockAsync(connection, cancellationToken);
        }
    }

    private async Task MigrateModuleAsync(
        IServiceScope scope,
        NpgsqlConnection connection,
        ModuleMigrationDescriptor descriptor,
        CancellationToken ct)
    {
        logger.LogInformation("Migrating module: {Module} (schema: {Schema})", descriptor.ModuleName, descriptor.SchemaName);

        // Ensure schema exists
        await EnsureSchemaExistsAsync(connection, descriptor.SchemaName, ct);

        // Ensure migrations history table exists in the module's schema
        await EnsureMigrationsHistoryTableExistsAsync(connection, descriptor.SchemaName, ct);

        // Resolve the module's DbContext and apply EF migrations
        var dbContext = ResolveDbContext(scope, descriptor);

        var pendingMigrations = (await dbContext.Database.GetPendingMigrationsAsync(ct)).ToList();
        var appliedMigrations = await dbContext.Database.GetAppliedMigrationsAsync(ct);

        logger.LogInformation(
            "Module {Module}: {AppliedCount} applied, {PendingCount} pending migration(s)",
            descriptor.ModuleName,
            appliedMigrations.Count(),
            pendingMigrations.Count);

        if (pendingMigrations.Count > 0)
        {
            logger.LogInformation("Applying {Count} pending migration(s) for module {Module}...",
                pendingMigrations.Count, descriptor.ModuleName);

            await dbContext.Database.MigrateAsync(ct);

            logger.LogInformation("Module {Module} EF migrations applied successfully", descriptor.ModuleName);
        }

        // Execute SQL scripts if the module has them
        if (descriptor.HasSqlScripts)
        {
            logger.LogInformation("Executing SQL scripts for module {Module}...", descriptor.ModuleName);

            var scriptExecutorOptions = new DatabaseScriptExecutorOptions
            {
                TrackingSchema = descriptor.SchemaName
            };

            var scriptSource = new FileSystemSqlScriptSource(
                descriptor.MigrationsAssembly,
                scriptExecutorOptions.ScriptsFolder,
                logger);

            var tracker = new PostgresSqlScriptTracker(scriptExecutorOptions);

            // Open a dedicated connection for script execution on the module's DbContext
            var moduleConnection = dbContext.Database.GetDbConnection();

            if (moduleConnection.State != ConnectionState.Open)
                await moduleConnection.OpenAsync(ct);

            if (moduleConnection is NpgsqlConnection npgsqlConn)
            {
                await tracker.EnsureCreatedAsync(npgsqlConn, ct);

                var scripts = await scriptSource.GetScriptsAsync(ct);
                if (scripts.Count > 0)
                {
                    var appliedScripts = await tracker.GetAppliedAsync(npgsqlConn, ct);

                    foreach (var script in scripts.OrderBy(s => s.FilePath))
                    {
                        if (appliedScripts.Contains(script.FileName))
                        {
                            logger.LogDebug("Script {Script} already applied for module {Module}, skipping",
                                script.FileName, descriptor.ModuleName);
                            continue;
                        }

                        logger.LogInformation("Executing script: {Script} for module {Module}",
                            script.FileName, descriptor.ModuleName);

                        await using var command = npgsqlConn.CreateCommand();
                        command.CommandText = script.Content.Trim();
                        command.CommandTimeout = scriptExecutorOptions.CommandTimeoutSeconds;

                        try
                        {
                            await command.ExecuteNonQueryAsync(ct);
                        }
                        catch (PostgresException ex)
                        {
                            logger.LogError(ex, "Failed executing script {Script} for module {Module}",
                                script.FileName, descriptor.ModuleName);
                            throw;
                        }

                        await tracker.MarkAppliedAsync(npgsqlConn, script.FileName, ct);
                        logger.LogInformation("Successfully executed script: {Script} for module {Module}",
                            script.FileName, descriptor.ModuleName);
                    }
                }
            }

            logger.LogInformation("SQL scripts processing completed for module {Module}", descriptor.ModuleName);
        }
    }

    private static DbContext ResolveDbContext(IServiceScope scope, ModuleMigrationDescriptor descriptor)
    {
        var dbContext = scope.ServiceProvider.GetService(descriptor.DbContextType) as DbContext
            ?? throw new InvalidOperationException(
                $"DbContext of type {descriptor.DbContextType.Name} is not registered for module {descriptor.ModuleName}");

        return dbContext;
    }

    private async Task EnsureDatabaseExistsAsync(string connectionString, CancellationToken ct)
    {
        var connectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionString);
        var databaseName = connectionStringBuilder.Database;

        if (string.IsNullOrWhiteSpace(databaseName))
            throw new InvalidOperationException("Database name is not specified in the connection string");

        var masterConnectionString = new NpgsqlConnectionStringBuilder(connectionString)
        {
            Database = "postgres"
        };

        await using var masterConnection = new NpgsqlConnection(masterConnectionString.ToString());
        await masterConnection.OpenAsync(ct);

        try
        {
            await using var checkCmd = masterConnection.CreateCommand();
            checkCmd.CommandText = """
				SELECT EXISTS(
					SELECT 1
					FROM pg_database
					WHERE datname = @databaseName
				);
				""";
            checkCmd.Parameters.Add(new NpgsqlParameter("databaseName", databaseName));
            var exists = await checkCmd.ExecuteScalarAsync(ct);

            if (exists is bool dbExists && dbExists)
            {
                logger.LogDebug("Database '{DatabaseName}' already exists", databaseName);
                return;
            }

            logger.LogInformation("Database '{DatabaseName}' does not exist. Creating it...", databaseName);
            await using var createCmd = masterConnection.CreateCommand();
            createCmd.CommandText = $"""CREATE DATABASE "{databaseName}" WITH ENCODING = 'UTF8' TEMPLATE = template0;""";
            await createCmd.ExecuteNonQueryAsync(ct);
            logger.LogInformation("Database '{DatabaseName}' created successfully", databaseName);
        }
        catch (PostgresException ex) when (ex.SqlState == "42P04")
        {
            logger.LogWarning("Database '{DatabaseName}' was created by another process", databaseName);
        }
    }

    private static async Task EnsureSchemaExistsAsync(NpgsqlConnection connection, string schema, CancellationToken ct)
    {
        await using var cmd = connection.CreateCommand();
        cmd.CommandText = $"""CREATE SCHEMA IF NOT EXISTS "{schema}";""";
        await cmd.ExecuteNonQueryAsync(ct);
    }

    private static async Task EnsureMigrationsHistoryTableExistsAsync(
        NpgsqlConnection connection,
        string schema,
        CancellationToken ct)
    {
        await using var checkCmd = connection.CreateCommand();
        checkCmd.CommandText = """
			SELECT EXISTS (
				SELECT 1
				FROM information_schema.tables
				WHERE table_schema = @schema
				AND table_name = @tableName
			);
			""";
        checkCmd.Parameters.Add(new NpgsqlParameter("schema", schema));
        checkCmd.Parameters.Add(new NpgsqlParameter("tableName", MigrationHistoryTableName));
        var tableExists = await checkCmd.ExecuteScalarAsync(ct);

        if (tableExists is true)
            return;

        await using var createCmd = connection.CreateCommand();
        createCmd.CommandText = $"""
			CREATE TABLE IF NOT EXISTS "{schema}"."{MigrationHistoryTableName}" (
				migration_id VARCHAR(150) NOT NULL,
				product_version VARCHAR(32) NOT NULL,
				CONSTRAINT "pk_{schema}_{MigrationHistoryTableName}" PRIMARY KEY (migration_id)
			);
			""";
        await createCmd.ExecuteNonQueryAsync(ct);
    }

    private async Task AcquireAdvisoryLockAsync(NpgsqlConnection connection, CancellationToken ct)
    {
        await using var lockCommand = connection.CreateCommand();
        lockCommand.CommandText = $"SELECT pg_advisory_lock({AdvisoryLockId})";
        await lockCommand.ExecuteNonQueryAsync(ct);
        logger.LogInformation("Acquired advisory lock for unified database migration");
    }

    private async Task ReleaseAdvisoryLockAsync(NpgsqlConnection connection, CancellationToken ct)
    {
        if (connection.State != ConnectionState.Open)
            return;

        try
        {
            await using var unlockCommand = connection.CreateCommand();
            unlockCommand.CommandText = $"SELECT pg_advisory_unlock({AdvisoryLockId})";
            await unlockCommand.ExecuteNonQueryAsync(ct);
            logger.LogInformation("Released advisory lock for unified database migration");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to release advisory lock");
        }
    }
}
