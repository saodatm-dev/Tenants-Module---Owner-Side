using Building.Application.Core.Abstractions.Data;
using Building.Domain;
using Building.Infrastructure.Database;
using Building.Infrastructure.Database.Seeds;
using Core.Application.Abstractions.Database;
using Core.Infrastructure.Database;
using Core.Infrastructure.Extensions;
using Core.Infrastructure.Database.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Building.Infrastructure.Extensions;

internal static class DatabaseExtensions
{
	extension(IServiceCollection services)
	{
		internal IServiceCollection AddDatabase(IConfiguration configuration)
		{
			var connectionString = configuration.GetConnectionString(Core.Infrastructure.Extensions.DatabaseExtensions.DatabaseName);
			ArgumentNullException.ThrowIfNullOrWhiteSpace(connectionString, nameof(connectionString));

			services.AddDbContext<BuildingDbContext>((serviceProvider, options) =>
			{
				options
					.UseNpgsql(connectionString, npgsqlOptions =>
					{
						npgsqlOptions.EnableRetryOnFailure();
						npgsqlOptions.MigrationsHistoryTable(MaydonDatabaseMigrator.MigrationHistoryTableName, AssemblyReference.Instance);
						npgsqlOptions.UseNetTopologySuite();
					})
					.UseAsyncSeeding((dbContext, _, cancellationToken) => Seed.SeedingAsync(serviceProvider, (IBuildingDbContext)dbContext, cancellationToken))
					.UseSnakeCaseNamingConvention()
					.EnableDetailedErrors()
					.ConfigureWarnings(item => item.Ignore(RelationalEventId.PendingModelChangesWarning))
					.AddInterceptors(
						serviceProvider.GetRequiredService<DomainEventInterceptor>(),
						serviceProvider.GetRequiredService<AuditableInterceptor>(),
						serviceProvider.GetRequiredService<SoftDeleteInterceptor>(),
						serviceProvider.GetRequiredService<ModifiableInterceptor>());
			});

			services.AddScoped<IBuildingDbContext>(sp => sp.GetRequiredService<BuildingDbContext>());

			services.AddModuleMigration(new ModuleMigrationDescriptor
			{
				ModuleName = "Building",
				SchemaName = AssemblyReference.Instance,
				DbContextType = typeof(BuildingDbContext),
				MigrationsAssembly = typeof(BuildingDbContext).Assembly,
				HasSqlScripts = false,
				Order = 3
			});

			return services;
		}
	}
}
