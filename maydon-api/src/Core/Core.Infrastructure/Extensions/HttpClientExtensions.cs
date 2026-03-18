using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Core.Infrastructure.Extensions;

internal static class HttpClientExtensions
{
	extension(IServiceCollection services)
	{
		internal IServiceCollection AddHttpClientConfiguration()
		{
			services.ConfigureHttpClientDefaults(configure =>
				configure.AddStandardResilienceHandler(options =>
				{
					options.Retry.Delay = TimeSpan.FromSeconds(1);
					options.Retry.MaxRetryAttempts = 3;

					options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(20);

					options.CircuitBreaker.FailureRatio = 0.2;

				}));

			return services;
		}
	}
	extension(IHttpClientBuilder builder)
	{
		public IHttpClientBuilder AddAuthToken()
		{
			builder.Services.AddHttpContextAccessor();

			builder.Services.TryAddTransient<HttpClientAuthorizationDelegatingHandler>();

			builder.AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>();

			return builder;
		}
	}

	private sealed class HttpClientAuthorizationDelegatingHandler : DelegatingHandler
	{
		private readonly IHttpContextAccessor _httpContextAccessor;

		public HttpClientAuthorizationDelegatingHandler(IHttpContextAccessor httpContextAccessor) =>
			_httpContextAccessor = httpContextAccessor;

		public HttpClientAuthorizationDelegatingHandler(IHttpContextAccessor httpContextAccessor, HttpMessageHandler innerHandler) : base(innerHandler)
			=> _httpContextAccessor = httpContextAccessor;

		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			if (_httpContextAccessor.HttpContext is HttpContext context)
			{
				var accessToken = await context.GetTokenAsync("access_token");

				if (accessToken is not null)
				{
					request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
				}
			}

			return await base.SendAsync(request, cancellationToken);
		}
	}
}
