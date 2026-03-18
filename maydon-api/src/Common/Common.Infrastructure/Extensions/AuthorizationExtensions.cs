using Common.Infrastructure.Authorization;
using Core.Infrastructure.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Infrastructure.Extensions;

internal static class AuthorizationExtensions
{
	extension(IServiceCollection services)
	{
		internal IServiceCollection AddAuthorizationInternal()
		{
			services.AddScoped<IPermissionProvider, CommonPermissionProvider>();

			return services;
		}
	}
}
