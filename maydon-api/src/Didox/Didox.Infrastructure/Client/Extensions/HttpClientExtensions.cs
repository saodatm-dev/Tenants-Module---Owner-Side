using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Didox.Infrastructure.Client.Extensions;

/// <summary>
/// Extension methods for configuring HTTP clients with resilience and authentication.
/// </summary>
internal static class HttpClientExtensions
{
    /// <summary>
    /// Configures default HTTP client settings with resilience handlers (retry, circuit breaker, timeout).
    /// </summary>
    /// <param name="services">The service collection</param>
    public static IServiceCollection AddHttpClientConfiguration(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.ConfigureHttpClientDefaults(configure =>
            configure.AddStandardResilienceHandler(options =>
            {
                // Retry configuration
                options.Retry.Delay = TimeSpan.FromSeconds(1);
                options.Retry.MaxRetryAttempts = 3;

                // Total request timeout (includes all retries)
                options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(20);

                // Circuit breaker configuration (opens after 20% failure rate)
                options.CircuitBreaker.FailureRatio = 0.2;
            }));

        return services;
    }

    /// <summary>
    /// Adds automatic JWT token forwarding from the current HTTP context to the HTTP client.
    /// </summary>
    /// <param name="builder">The HTTP client builder</param>
    public static IHttpClientBuilder AddAuthToken(this IHttpClientBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        
        builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        return builder;
    }
}

