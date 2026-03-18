using Common.Infrastructure.Extensions;
using Identity.Infrastructure.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Infrastructure;

public static class DependencyInjection
{
	extension(IServiceCollection services)
	{
		public IServiceCollection AddIdentityInfrastructure(IConfiguration configuration)
		{
			services.AddOptionsInternal()
				.AddHttpClients()
				.AddAuthenticationInternal()
				.AddAuthorizationInternal()
				.AddDatabase(configuration);

			return services;
		}
	}
}
