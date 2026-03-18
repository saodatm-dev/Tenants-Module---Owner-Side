using Core.Infrastructure.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Infrastructure;

public static class DependencyInjection
{
	extension(IServiceCollection services)
	{
		public IServiceCollection AddCoreInfrastructure(IConfiguration configuration) =>
			services.AddOptionsInternal()
				.AddHttpClientConfiguration()
				.AddAuthenticationInternal()
				.AddAuthorizationInternal()
				.AddDatabase(configuration)
				.AddServicesInternal()
				.AddRedisInfrastructure(configuration)
				.AddIntegrationEvents(configuration)
				.AddResilienceInfrastructure()
				.AddEncryption(configuration)
				.AddVersioning(configuration)
				.AddFusionCacheDefaults(configuration);
	}
}
