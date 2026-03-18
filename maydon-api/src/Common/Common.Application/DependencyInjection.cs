using Microsoft.Extensions.DependencyInjection;

namespace Common.Application;

public static class DependencyInjection
{
	public static IServiceCollection AddCommonApplication(this IServiceCollection services)
	{
		return services;
	}
}
