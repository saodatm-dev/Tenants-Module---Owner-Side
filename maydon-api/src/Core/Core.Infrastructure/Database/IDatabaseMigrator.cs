using Microsoft.Extensions.DependencyInjection;

namespace Core.Infrastructure.Database;

public interface IDatabaseMigrator
{
	Task MigrateAsync(IServiceScope scope, CancellationToken cancellationToken = default);
}
