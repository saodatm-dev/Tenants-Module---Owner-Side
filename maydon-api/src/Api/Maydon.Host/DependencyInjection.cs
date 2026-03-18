using Maydon.Host.Extensions;
using Maydon.Host.Infrastructure;

namespace Maydon.Host;

public static class DependencyInjection
{
	extension(IServiceCollection services)
	{
		public IServiceCollection AddPresentation(IConfiguration configuration)
		{
			services.AddVersioning()
				.AddEndpoints()
				.AddAntiforgery()
				.AddLocalizationInternal()
				.AddSwaggerInternal()
				.AddCorsInternal(configuration)
				.AddRateLimiterInternal();

			services.AddExceptionHandler<GlobalExceptionHandler>();
			services.AddProblemDetails();

			return services;
		}
	}

	extension(WebApplication app)
	{
		public WebApplication UsePresentation()
		{
			app.MapEndpoints()
				.UseSwaggerInternal()
				.UseAntiforgery()
				.UseSecurityHeader()
				.UseExceptionHandler()
				.UseRequestLocalization()
				.UseCors()
				.UseCookiePolicy();

			return app;
		}
	}
}
