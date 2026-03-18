using Core.Application.Abstractions.Database;
using Core.Infrastructure.Database;
using Core.Infrastructure.Database.Interceptors;
using Core.Infrastructure.Extensions;
using Identity.Application.Core.Abstractions.Data;
using Identity.Domain;
using Identity.Infrastructure.Database;
using Identity.Infrastructure.Database.Seeds;
using Microsoft.EntityFrameworkCore;
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

			services.AddDbContext<IdentityDbContext>((serviceProvider, options) =>
			{
				options
					.UseNpgsql(connectionString, npgsqlOptions =>
					{
						npgsqlOptions.EnableRetryOnFailure();
						npgsqlOptions.MigrationsHistoryTable(MaydonDatabaseMigrator.MigrationHistoryTableName, AssemblyReference.Instance);
					})
					.ConfigureWarnings(warnings => warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning))
					.UseAsyncSeeding((dbContext, _, cancellationToken) => Seed.SeedingAsync(serviceProvider, (IIdentityDbContext)dbContext, cancellationToken))
					.UseSnakeCaseNamingConvention()
					.EnableDetailedErrors()
					.AddInterceptors(
						serviceProvider.GetRequiredService<DomainEventInterceptor>(),
						serviceProvider.GetRequiredService<AuditableInterceptor>(),
						serviceProvider.GetRequiredService<SoftDeleteInterceptor>(),
						serviceProvider.GetRequiredService<ModifiableInterceptor>());
			});

			services.TryAddScoped<IIdentityDbContext>(sp => sp.GetRequiredService<IdentityDbContext>());

			services.AddModuleMigration(new ModuleMigrationDescriptor
			{
				ModuleName = "Identity",
				SchemaName = AssemblyReference.Instance,
				DbContextType = typeof(IdentityDbContext),
				MigrationsAssembly = typeof(IdentityDbContext).Assembly,
				HasSqlScripts = false,
				Order = 2
			});

			return services;
		}
	}
}
