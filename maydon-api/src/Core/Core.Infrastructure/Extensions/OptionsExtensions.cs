using Core.Infrastructure.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Infrastructure.Extensions;

internal static class OptionsExtensions
{
	extension(IServiceCollection services)
	{
		internal IServiceCollection AddOptionsInternal()
		{
			services.AddOptions<JwtOptions>()
				.BindConfiguration(nameof(JwtOptions))
				.ValidateDataAnnotations()
				.ValidateOnStart();

			services.AddOptions<CoreOptions>()
				.BindConfiguration(nameof(CoreOptions))
				.ValidateDataAnnotations()
				.ValidateOnStart();

			return services;
		}
	}
}
