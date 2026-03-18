using Building.Infrastructure.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Building.Infrastructure;

public static class DependencyInjection
{
	extension(IServiceCollection services)
	{
		public IServiceCollection AddBuildingInfrastructure(IConfiguration configuration)
		{
			services.AddOptionsInternal()
				.AddHttpClients()
				.AddAuthorizationInternal()
				.AddDatabase(configuration)
				.AddGeometryService();

			return services;
		}
	}
}
