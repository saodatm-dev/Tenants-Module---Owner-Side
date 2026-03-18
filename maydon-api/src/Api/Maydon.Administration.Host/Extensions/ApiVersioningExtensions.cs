using Asp.Versioning;

namespace Maydon.Administration.Host.Extensions;

internal static class ApiVersioningExtensions
{
	extension(IServiceCollection services)
	{
		internal IServiceCollection AddVersioning()
		{
			services.AddApiVersioning(options =>
			{
				options.DefaultApiVersion = new ApiVersion(1);
				options.ReportApiVersions = true;
				options.AssumeDefaultVersionWhenUnspecified = true;
				options.ApiVersionReader = ApiVersionReader.Combine(
					new UrlSegmentApiVersionReader(),
					//new QueryStringApiVersionReader("api-version"),
					new HeaderApiVersionReader("X-Api-Version"));

			})
			.AddApiExplorer(options =>
			{
				options.GroupNameFormat = "'v'V";
				options.SubstituteApiVersionInUrl = false;
			});

			return services;
		}
	}
}
