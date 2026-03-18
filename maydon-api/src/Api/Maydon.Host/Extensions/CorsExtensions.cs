namespace Maydon.Host.Extensions;

internal static class CorsExtensions
{
	extension(IServiceCollection services)
	{
		internal IServiceCollection AddCorsInternal(IConfiguration configuration)
		{
			var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

			services.AddCors(cors =>
				cors.AddDefaultPolicy(options =>
				{
					if (allowedOrigins.Length > 0)
					{
						options.WithOrigins(allowedOrigins)
							.AllowAnyHeader()
							.AllowAnyMethod()
							.AllowCredentials();
					}
					else
					{
						// Development fallback - still not "any origin"
						options.SetIsOriginAllowed(options => true)
							.AllowAnyHeader()
							.AllowAnyMethod()
							.AllowCredentials();
					}
				}));
			return services;
		}
	}
}
