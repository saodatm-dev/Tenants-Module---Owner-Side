using Building.Infrastructure.Authorization;
using Core.Infrastructure.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Building.Infrastructure.Extensions;

internal static class AuthorizationExtensions
{
	extension(IServiceCollection services)
	{
		internal IServiceCollection AddAuthorizationInternal()
		{
			services.AddScoped<IPermissionProvider, BuildingPermissionProvider>();

			return services;
		}
	}
}
