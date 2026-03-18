using Core.Application.Abstractions.Database;
using Core.Infrastructure.Database;
using Core.Infrastructure.Database.Interceptors;
using Core.Infrastructure.DomainEvents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Core.Infrastructure.Extensions;

public static class DatabaseExtensions
{
	public const string DatabaseName = "maydondb";

	extension(IServiceCollection services)
	{
		internal IServiceCollection AddDatabase(IConfiguration configuration)
		{
			var connectionString = configuration.GetConnectionString(DatabaseName);
			ArgumentNullException.ThrowIfNullOrWhiteSpace(connectionString, nameof(connectionString));

			services.AddHealthChecks()
				.AddNpgSql(connectionString);

			services.TryAddScoped<IDomainEventPublisher, DomainEventPublisher>();
			services.TryAddScoped<DomainEventInterceptor>();
			services.TryAddSingleton<AuditableInterceptor>();
			services.TryAddSingleton<SoftDeleteInterceptor>();
			services.TryAddScoped<ModifiableInterceptor>();

			// Unified migration infrastructure
			services.TryAddSingleton<IModuleMigrationRegistry, ModuleMigrationRegistry>();
			services.TryAddScoped<IDatabaseMigrator, MaydonDatabaseMigrator>();

			return services;
		}

		/// <summary>
		/// Registers a module's migration descriptor into the shared registry.
		/// Call this from each module's DI setup.
		/// </summary>
		public IServiceCollection AddModuleMigration(ModuleMigrationDescriptor descriptor)
		{
			// Build a temporary provider to register into the singleton registry
			// Use a post-configure approach: register a hosted initializer
			services.AddSingleton(descriptor);
			return services;
		}
	}

	extension(IServiceScope scope)
	{
		public async Task MigrateDatabasesAsync(CancellationToken cancellationToken = default)
		{
			// Populate registry from all registered descriptors
			var registry = scope.ServiceProvider.GetRequiredService<IModuleMigrationRegistry>();
			var descriptors = scope.ServiceProvider.GetServices<ModuleMigrationDescriptor>();

			foreach (var descriptor in descriptors)
				registry.Register(descriptor);

			var migrator = scope.ServiceProvider.GetRequiredService<IDatabaseMigrator>();
			await migrator.MigrateAsync(scope, cancellationToken);
		}
	}
}
