using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;

namespace Core.Infrastructure.Resilience;

/// <summary>
/// Centralized Polly resilience policies for external system integration.
/// Provides retry, circuit breaker, timeout, and fallback patterns.
/// </summary>
public static class PollyPolicies
{
    /// <summary>
    /// Retry policy for transient network failures with exponential backoff and jitter
    /// </summary>
    public static AsyncRetryPolicy CreateNetworkRetryPolicy(int maxRetryAttempts = 3, ILogger? logger = null)
    {
        var random = new Random();
        
        return Policy
            .Handle<HttpRequestException>()
            .Or<TimeoutException>()
            .Or<TaskCanceledException>()
            .WaitAndRetryAsync(
                retryCount: maxRetryAttempts,
                sleepDurationProvider: retryAttempt =>
                {
                    // Exponential backoff: 2^retry * 1000ms
                    var exponentialDelay = TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
                    
                    // Add jitter: +/- 20%
                    var jitter = TimeSpan.FromMilliseconds(
                        exponentialDelay.TotalMilliseconds * 0.2 * (random.NextDouble() - 0.5) * 2);
                    
                    return exponentialDelay + jitter;
                },
                onRetry: (exception, timeSpan, retryCount, _) =>
                {
                    logger?.LogWarning(exception,
                        "Network retry {RetryCount}/{MaxRetries} after {Delay}ms - {ExceptionType}: {Message}",
                        retryCount, maxRetryAttempts, timeSpan.TotalMilliseconds, 
                        exception.GetType().Name, exception.Message);
                });
    }

    /// <summary>
    /// Retry policy for database transient failures (deadlocks, timeouts)
    /// </summary>
    public static AsyncRetryPolicy CreateDatabaseRetryPolicy(int maxRetryAttempts = 5, ILogger? logger = null)
    {
        return Policy
            .Handle<InvalidOperationException>(ex => 
                ex.Message.Contains("deadlock", StringComparison.OrdinalIgnoreCase) ||
                ex.Message.Contains("timeout", StringComparison.OrdinalIgnoreCase))
            .Or<TimeoutException>()
            .WaitAndRetryAsync(
                retryCount: maxRetryAttempts,
                sleepDurationProvider: retryAttempt => TimeSpan.FromMilliseconds(100 * retryAttempt),
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    logger?.LogWarning(exception,
                        "Database retry {RetryCount}/{MaxRetries} after {Delay}ms",
                        retryCount, maxRetryAttempts, timeSpan.TotalMilliseconds);
                });
    }

    /// <summary>
    /// Circuit breaker to prevent cascading failures to unhealthy services
    /// Note: Using advanced circuit breaker for Polly v8 compatibility
    /// </summary>
    private static AsyncCircuitBreakerPolicy CreateCircuitBreakerPolicy(int failureThreshold = 5, TimeSpan? durationOfBreak = null, ILogger? logger = null)
    {
        durationOfBreak ??= TimeSpan.FromSeconds(30);
        
        return Policy
            .Handle<HttpRequestException>()
            .Or<TimeoutException>()
            .AdvancedCircuitBreakerAsync(
                failureThreshold: 0.5, // 50% failure rate
                samplingDuration: TimeSpan.FromSeconds(10),
                minimumThroughput: failureThreshold,
                durationOfBreak: durationOfBreak.Value,
                onBreak: (exception, duration) =>
                {
                    logger?.LogError(exception,
                        "Circuit breaker opened for {Duration}s after failure threshold reached",
                        duration.TotalSeconds);
                },
                onReset: () =>
                {
                    logger?.LogInformation("Circuit breaker reset - service is healthy again");
                },
                onHalfOpen: () =>
                {
                    logger?.LogInformation("Circuit breaker half-open - testing service health");
                });
    }

    /// <summary>
    /// Timeout policy to prevent hanging operations
    /// </summary>
    private static AsyncTimeoutPolicy CreateTimeoutPolicy(TimeSpan timeout, ILogger? logger = null)
    {
        return Policy
            .TimeoutAsync(
                timeout,
                TimeoutStrategy.Optimistic,
                onTimeoutAsync: (context, timeSpan, task) =>
                {
                    logger?.LogWarning(
                        "Operation timed out after {Timeout}s",
                        timeSpan.TotalSeconds);
                    return Task.CompletedTask;
                });
    }

    /// <summary>
    /// Fallback policy to provide default behavior on failure
    /// </summary>
    public static AsyncPolicy<T> CreateFallbackPolicy<T>(T fallbackValue, ILogger? logger = null)
    {
        return Policy<T>
            .Handle<Exception>()
            .FallbackAsync(
                fallbackValue,
                onFallbackAsync: (result, context) =>
                {
                    logger?.LogWarning(result.Exception,
                        "Fallback executed due to: {ExceptionType}",
                        result.Exception?.GetType().Name);
                    return Task.CompletedTask;
                });
    }

    /// <summary>
    /// Combined policy (wrap) for comprehensive resilience:
    /// Timeout → Retry → Circuit Breaker
    /// </summary>
    public static IAsyncPolicy CreateComprehensivePolicy(
        TimeSpan timeout,
        int maxRetries = 3,
        int circuitBreakerFailureThreshold = 5,
        ILogger? logger = null)
    {
        var timeoutPolicy = CreateTimeoutPolicy(timeout, logger);
        var retryPolicy = CreateNetworkRetryPolicy(maxRetries, logger);
        var circuitBreakerPolicy = CreateCircuitBreakerPolicy(circuitBreakerFailureThreshold, null, logger);
        
        return Policy.WrapAsync(timeoutPolicy, retryPolicy, circuitBreakerPolicy);
    }

    /// <summary>
    /// Bulkhead isolation policy to limit concurrent operations
    /// </summary>
    public static AsyncPolicy CreateBulkheadPolicy(int maxParallelization = 10, int maxQueuingActions = 25, ILogger? logger = null)
    {
        return Policy.BulkheadAsync(
            maxParallelization,
            maxQueuingActions,
            onBulkheadRejectedAsync: context =>
            {
                logger?.LogWarning(
                    "Bulkhead rejected execution - {Parallelization} operations in progress, {Queued} queued",
                    maxParallelization, maxQueuingActions);
                return Task.CompletedTask;
            });
    }
}
