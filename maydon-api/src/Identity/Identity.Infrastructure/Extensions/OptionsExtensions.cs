using Identity.Infrastructure.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Infrastructure.Extensions;

internal static class OptionsExtensions
{
	extension(IServiceCollection services)
	{
		internal IServiceCollection AddOptionsInternal()
		{
			services.AddOptions<EImzoOptions>()
				.BindConfiguration(nameof(EImzoOptions))
				.ValidateDataAnnotations()
				.ValidateOnStart();

			services.AddOptions<OneIdOptions>()
				.BindConfiguration(nameof(OneIdOptions))
				.ValidateDataAnnotations()
				.ValidateOnStart();

			services.AddOptions<OtpOptions>()
				.BindConfiguration(nameof(OtpOptions))
				.ValidateDataAnnotations()
				.ValidateOnStart();

			return services;
		}
	}
}
