using Core.Infrastructure.Resilience;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Core.Infrastructure.Extensions;

/// <summary>
/// Extension methods for configuring production-grade resilience patterns.
/// </summary>
public static class ResilienceExtensions
{
    /// <summary>
    /// Adds comprehensive resilience infrastructure:
    /// Polly policies (retry, circuit breaker, timeout, bulkhead).
    /// </summary>
    public static IServiceCollection AddResilienceInfrastructure(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        
        services.AddSingleton<IPolicyRegistry>(sp =>
        {
            var registry = new PolicyRegistry();
            var logger = sp.GetRequiredService<ILogger<PolicyRegistry>>();
            
            registry.RegisterPolicy(
                PolicyNames.ExternalApiCall,
                PollyPolicies.CreateComprehensivePolicy(
                    timeout: TimeSpan.FromSeconds(30),
                    maxRetries: 3,
                    circuitBreakerFailureThreshold: 5,
                    logger: logger));

            registry.RegisterPolicy(
                PolicyNames.DatabaseOperation,
                PollyPolicies.CreateDatabaseRetryPolicy(5, logger));

            registry.RegisterPolicy(
                PolicyNames.MessagePublishing,
                PollyPolicies.CreateNetworkRetryPolicy(3, logger));

            registry.RegisterPolicy(
                PolicyNames.DidoxApiCall,
                PollyPolicies.CreateComprehensivePolicy(
                    timeout: TimeSpan.FromSeconds(60),
                    maxRetries: 5,
                    circuitBreakerFailureThreshold: 10,
                    logger: logger));

            return registry;
        });

        return services;
    }
}
