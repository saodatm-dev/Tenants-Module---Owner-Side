using Core.Infrastructure.Authorization;
using Identity.Infrastructure.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Infrastructure.Extensions;

internal static class AuthorizationExtensions
{
	extension(IServiceCollection services)
	{
		internal IServiceCollection AddAuthorizationInternal()
		{
			services.AddScoped<IPermissionProvider, IdentityPermissionProvider>();

			services.AddScoped<ISessionProvider, IdentitySessionProvider>();
			return services;
		}
	}
}
