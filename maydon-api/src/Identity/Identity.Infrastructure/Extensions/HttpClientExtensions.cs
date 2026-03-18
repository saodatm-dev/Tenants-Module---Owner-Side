using System.Net.Http.Headers;
using System.Text;
using Identity.Application.Core.Abstractions.Services.EImzo;
using Identity.Application.Core.Abstractions.Services.OneId;
using Identity.Application.Core.Abstractions.Services.Otp;
using Identity.Infrastructure.Options;
using Identity.Infrastructure.Services.EImzo;
using Identity.Infrastructure.Services.OneId;
using Identity.Infrastructure.Services.Otp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Identity.Infrastructure.Extensions;

internal static class HttpClientExtensions
{
	extension(IServiceCollection services)
	{
		internal IServiceCollection AddHttpClients()
		{
			const string BasicAuthenticationHeaderValue = "Basic";

			services.AddHttpClient<IEImzoService, EImzoService>((serviceProvider, httpClient) =>
			{
				var eImzoOptions = serviceProvider.GetRequiredService<IOptions<EImzoOptions>>().Value;

				//httpClient.DefaultRequestHeaders.Add("Owner", eImzoOptions.RegisteredHost);
				//httpClient.DefaultRequestHeaders.Add("X-Real-IP", eImzoOptions.XRealIP);

				httpClient.BaseAddress = new Uri(eImzoOptions.Host);// $"{(eImzoOptions.IsHttps ? "https://" : "http://")}{eImzoOptions.Owner}:{eImzoOptions.Port}");

			}).AddStandardResilienceHandler();

			services.AddHttpClient<IOneIdService, OneIdService>((serviceProvider, httpClient) =>
			{
				var oneIdOptions = serviceProvider.GetRequiredService<IOptions<OneIdOptions>>().Value;

				httpClient.BaseAddress = new Uri(oneIdOptions.Uri);

			}).AddStandardResilienceHandler();

			services.AddHttpClient<IOtpService, OtpService>((serviceProvider, httpClient) =>
			{
				var otpOptions = serviceProvider.GetRequiredService<IOptions<OtpOptions>>().Value;

				httpClient.BaseAddress = new Uri(otpOptions.Uri);
				httpClient.DefaultRequestHeaders.Authorization =
				new AuthenticationHeaderValue(
					BasicAuthenticationHeaderValue,
					Convert.ToBase64String(Encoding.UTF8.GetBytes($"{otpOptions.ClientId}:{otpOptions.Secret}")));

			}).AddStandardResilienceHandler();

			return services;
		}
	}
}
