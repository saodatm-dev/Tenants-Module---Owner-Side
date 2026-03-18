using Core.Infrastructure.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Infrastructure.Extensions;

internal static class AuthorizationExtensions
{
	extension(IServiceCollection services)
	{
		internal IServiceCollection AddAuthorizationInternal()
		{
			services.AddAuthorization();

			services.AddScoped<IAuthorizationHandler, SessionAuthorizationHandler>();

			services.AddScoped<PermissionProvider>();

			services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();

			services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();

			return services;
		}
	}
}
