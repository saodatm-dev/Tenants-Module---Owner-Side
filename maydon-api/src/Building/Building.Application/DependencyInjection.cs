using Microsoft.Extensions.DependencyInjection;

namespace Building.Application;

public static class DependencyInjection
{
	public static IServiceCollection AddBuildingApplication(this IServiceCollection services)
	{
		return services;
	}
}
