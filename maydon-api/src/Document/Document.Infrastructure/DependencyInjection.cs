using Document.Application.Abstractions.Data;
using Core.Application.Abstractions.Database;
using Core.Application.Abstractions.Services;
using Core.Infrastructure.Database;
using Core.Infrastructure.Extensions;
using Core.Infrastructure.Versioning;
using Core.Infrastructure.Configuration;
using Document.Contract.Contracts;
using Document.Contract.Gateways;
using Document.Contract.SignalR;
using Document.Infrastructure.Services;
using Document.Infrastructure.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Document.Infrastructure;

/// <summary>
/// Dependency injection configuration for Document Infrastructure layer.
/// </summary>
public static class DependencyInjection
{
    private const string ConnectionStringName = Core.Infrastructure.Extensions.DatabaseExtensions.DatabaseName;

    extension(IServiceCollection services)
    {
        /// <summary>
        /// Adds Document Infrastructure services to the service collection.
        /// </summary>
        public IServiceCollection AddDocumentInfrastructure(IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(configuration);

            services.AddPooledDbContextFactory<DocumentDbContext>(options =>
            {
                var connectionString = configuration.GetConnectionString(ConnectionStringName)
                                       ?? throw new InvalidOperationException($"Connection string '{ConnectionStringName}' is not configured.");

                options.UseNpgsql(
                        connectionString,
                        npgsqlOptions =>
                        {
                            npgsqlOptions.MigrationsAssembly(typeof(DocumentDbContext).Assembly.FullName);
                            npgsqlOptions.CommandTimeout(30);
                            npgsqlOptions.MigrationsHistoryTable(MaydonDatabaseMigrator.MigrationHistoryTableName, DocumentDbContext.SchemaName);
                            npgsqlOptions.EnableRetryOnFailure(3);
                        })
                    .UseSnakeCaseNamingConvention();
            });

            services.AddScoped<DocumentDbContext>(sp => sp.GetRequiredService<IDbContextFactory<DocumentDbContext>>().CreateDbContext());

            services.AddScoped<IDocumentDbContext>(sp => sp.GetRequiredService<DocumentDbContext>());

            services.AddScoped<IDocumentStatusNotifier, DocumentStatusNotifier>();

            services.AddScoped<IUserLookupService, UserLookupService>();

            services.AddScoped<IOwnerPlaceholderResolver, OwnerPlaceholderResolver>();

            services.AddScoped<IBuildingReadGateway, BuildingReadGateway>();

            services.AddScoped<IContractPlaceholderResolver, ContractPlaceholderResolver>();

            services.AddScoped<IContractNumberGenerator, ContractNumberGenerator>();

            services.AddModuleMigration(new ModuleMigrationDescriptor
            {
                ModuleName = "Document",
                SchemaName = DocumentDbContext.SchemaName,
                DbContextType = typeof(DocumentDbContext),
                MigrationsAssembly = typeof(DocumentDbContext).Assembly,
                HasSqlScripts = true,
                Order = 4
            });

            services.AddOptions<RedisOptions>()
                .Bind(configuration.GetSection(RedisOptions.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            // Background jobs
            services.AddHostedService<Jobs.ExpireContractsJob>();

            // Versioning services
            services.AddScoped<IVersioningService, VersioningService>();
            services.AddScoped<IVersionDiffService, VersionDiffService>();

            return services;
        }
    }
}

