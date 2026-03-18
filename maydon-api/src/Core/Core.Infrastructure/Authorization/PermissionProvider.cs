using Microsoft.Extensions.DependencyInjection;

namespace Core.Infrastructure.Authorization;

internal sealed class PermissionProvider(IServiceProvider serviceProvider)
{
	public async ValueTask<List<string>> GetByRoleIdAsync(Guid roleId)
	{
		using var scope = serviceProvider.CreateAsyncScope();

		var permissionProviders = scope.ServiceProvider.GetRequiredService<IEnumerable<IPermissionProvider>>();

		var permissions = new List<string>();

		foreach (var permissionProvider in permissionProviders)
			permissions.AddRange(await permissionProvider.GetByRoleIdAsync(roleId));

		return permissions;
	}
}
