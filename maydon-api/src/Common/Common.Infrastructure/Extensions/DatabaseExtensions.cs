using Common.Application.Core.Abstractions.Data;
using Common.Domain;
using Common.Infrastructure.Database;
using Common.Infrastructure.Database.Seeds;
using Core.Application.Abstractions.Database;
using Core.Infrastructure.Database;
using Core.Infrastructure.Extensions;
using Core.Infrastructure.Database.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Common.Infrastructure.Extensions;

internal static class DatabaseExtensions
{
	extension(IServiceCollection services)
	{
		internal IServiceCollection AddDatabase(IConfiguration configuration)
		{
			var connectionString = configuration.GetConnectionString(Core.Infrastructure.Extensions.DatabaseExtensions.DatabaseName);
			ArgumentNullException.ThrowIfNullOrWhiteSpace(connectionString, nameof(connectionString));

			services.AddDbContext<CommonDbContext>((serviceProvider, options) =>
			{
				options
					.UseNpgsql(connectionString, npgsqlOptions =>
					{
						npgsqlOptions.EnableRetryOnFailure();
						npgsqlOptions.MigrationsHistoryTable(MaydonDatabaseMigrator.MigrationHistoryTableName, AssemblyReference.Instance);
					})
					.UseAsyncSeeding((dbContext, _, cancellationToken) => Seed.SeedingAsync(serviceProvider, (ICommonDbContext)dbContext, cancellationToken))
					.UseSnakeCaseNamingConvention()
					.EnableDetailedErrors()
					.ConfigureWarnings(item => item.Ignore(RelationalEventId.PendingModelChangesWarning))
					.AddInterceptors(
						serviceProvider.GetRequiredService<DomainEventInterceptor>(),
						serviceProvider.GetRequiredService<AuditableInterceptor>(),
						serviceProvider.GetRequiredService<SoftDeleteInterceptor>(),
						serviceProvider.GetRequiredService<ModifiableInterceptor>());
			});

			services.TryAddScoped<ICommonDbContext>(sp => sp.GetRequiredService<CommonDbContext>());

			services.AddModuleMigration(new ModuleMigrationDescriptor
			{
				ModuleName = "Common",
				SchemaName = AssemblyReference.Instance,
				DbContextType = typeof(CommonDbContext),
				MigrationsAssembly = typeof(CommonDbContext).Assembly,
				HasSqlScripts = false,
				Order = 1
			});

			return services;
		}
	}
}
