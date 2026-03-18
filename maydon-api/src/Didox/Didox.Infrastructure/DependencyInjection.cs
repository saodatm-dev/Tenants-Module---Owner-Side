using Core.Application.Abstractions.Database;
using Core.Application.Abstractions.Messaging;
using Core.Infrastructure.Database;
using Core.Infrastructure.Extensions;
using Core.Infrastructure.Messaging;
using Core.Infrastructure.Pdf;
using Didox.Application.Abstractions.Database;
using Didox.Infrastructure.Workers;
using Document.Contract.IntegrationEvents;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Didox.Infrastructure;

/// <summary>
/// Dependency injection configuration for Didox Infrastructure layer
/// </summary>
public static class DependencyInjection
{
    private const string ConnectionStringName = Core.Infrastructure.Extensions.DatabaseExtensions.DatabaseName;

    /// <summary>
    /// Adds Didox Infrastructure services to the service collection.
    /// Registers DbContext and database migrators.
    /// </summary>
    public static IServiceCollection AddDidoxInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddPooledDbContextFactory<DidoxDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString(ConnectionStringName)
                                   ?? throw new InvalidOperationException($"Connection string '{ConnectionStringName}' is not configured.");

            options.UseNpgsql(
                    connectionString,
                    npgsqlOptions =>
                    {
                        npgsqlOptions.MigrationsAssembly(typeof(DidoxDbContext).Assembly.FullName);
                        npgsqlOptions.CommandTimeout(30);
                        npgsqlOptions.MigrationsHistoryTable(MaydonDatabaseMigrator.MigrationHistoryTableName, DidoxDbContext.SchemaName);
                        npgsqlOptions.EnableRetryOnFailure(3);
                    })
                .UseSnakeCaseNamingConvention();
        });

        services.AddScoped<DidoxDbContext>(sp =>
            sp.GetRequiredService<IDbContextFactory<DidoxDbContext>>().CreateDbContext());

        services.AddScoped<IDidoxDbContext>(sp => sp.GetRequiredService<DidoxDbContext>());

        services.AddModuleMigration(new ModuleMigrationDescriptor
        {
            ModuleName = "Didox",
            SchemaName = DidoxDbContext.SchemaName,
            DbContextType = typeof(DidoxDbContext),
            MigrationsAssembly = typeof(DidoxDbContext).Assembly,
            HasSqlScripts = true,
            Order = 5
        });

        // Register the export event handler so the pipeline can dispatch export events.
        // This handler lives in Didox.Infrastructure, not an application assembly.
        services.AddScoped<IIntegrationEventHandler<DocumentExportRequested>, DidoxExportWorker>();

        return services;
    }

    public static IServiceCollection AddDidoxExternalDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        // PDF rendering will be configured when the service is available in maydon-api
        return services;
    }
}

