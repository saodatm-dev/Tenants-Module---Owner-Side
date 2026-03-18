using Core.Application.Abstractions.Messaging;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace Core.Infrastructure.Messaging.Behaviors;

/// <summary>
/// Retry decorator for integration event handlers.
/// Implements automatic retry with exponential backoff for transient failures.
/// Note: This is handler-level retry (in-process), separate from infrastructure-level retry.
/// </summary>
public sealed class RetryBehavior : IGlobalIntegrationEventBehavior
{
    private readonly ILogger<RetryBehavior> _logger;
    private readonly AsyncRetryPolicy _retryPolicy;

    public RetryBehavior(ILogger<RetryBehavior> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        _retryPolicy = Policy
            .Handle<TimeoutException>()
            .Or<HttpRequestException>()
            .WaitAndRetryAsync(
                retryCount: 2, // Handler-level: fewer retries (infrastructure handles more)
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (exception, timeSpan, retryCount, _) =>
                {
                    _logger.LogWarning(exception,
                        "Handler retry {RetryCount} after {Delay}s due to {ExceptionType}",
                        retryCount, timeSpan.TotalSeconds, exception.GetType().Name);
                });
    }

    public async Task HandleAsync(
        IIntegrationEvent @event, 
        Func<Task> next, 
        CancellationToken cancellationToken)
    {
        await _retryPolicy.ExecuteAsync(async () => await next());
    }
}
