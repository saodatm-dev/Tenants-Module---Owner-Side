using Building.Infrastructure.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Building.Infrastructure.Extensions;

internal static class OptionsExtensions
{
	extension(IServiceCollection services)
	{
		internal IServiceCollection AddOptionsInternal()
		{
			services.AddOptions<MinioApiOptions>()
				.BindConfiguration(nameof(MinioApiOptions))
				.ValidateDataAnnotations()
				.ValidateOnStart();

			return services;
		}
	}
}
