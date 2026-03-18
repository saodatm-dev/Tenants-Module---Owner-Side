using Common.Infrastructure.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Infrastructure;

public static class DependencyInjection
{
	extension(IServiceCollection services)
	{
		public IServiceCollection AddCommonInfrastructure(IConfiguration configuration)
		{
			services.AddAuthorizationInternal()
				.AddDatabase(configuration);

			return services;
		}
	}
}
