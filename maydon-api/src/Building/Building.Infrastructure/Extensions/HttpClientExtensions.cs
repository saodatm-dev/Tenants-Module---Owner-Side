using Microsoft.Extensions.DependencyInjection;

namespace Building.Infrastructure.Extensions;

internal static class HttpClientExtensions
{
	extension(IServiceCollection services)
	{
		internal IServiceCollection AddHttpClients()
		{
			//services.AddHttpClient<IMinioApiService, MinioApiService>((serviceProvider, httpClient) =>
			//{
			//	var minIOOptions = serviceProvider.GetRequiredService<IOptions<MinioApiOptions>>().Value;

			//	httpClient.BaseAddress = new Uri(minIOOptions.BaseUrl);

			//}).AddStandardResilienceHandler();

			return services;
		}
	}
}
