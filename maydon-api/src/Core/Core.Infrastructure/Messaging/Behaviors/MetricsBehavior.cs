using System.Diagnostics;
using Core.Application.Abstractions.Messaging;
using Core.Infrastructure.Monitoring;

namespace Core.Infrastructure.Messaging.Behaviors;

/// <summary>
/// Metrics decorator for integration event handlers.
/// Records metrics for monitoring and observability.
/// </summary>
public sealed class MetricsBehavior(IntegrationEventMetrics metrics) : IGlobalIntegrationEventBehavior
{
    private readonly IntegrationEventMetrics _metrics = metrics ?? throw new ArgumentNullException(nameof(metrics));

    public async Task HandleAsync(IIntegrationEvent @event, Func<Task> next, CancellationToken cancellationToken)
    {
        var eventType = @event.GetType().Name;
        var stopwatch = Stopwatch.StartNew();
        var occurredOn = @event.OccurredOn;

        try
        {
            _metrics.IncrementActiveHandlers();
            
            await next();
            
            stopwatch.Stop();
            var endToEndLatency = (DateTime.UtcNow - occurredOn).TotalMilliseconds;
            
            _metrics.RecordEventProcessed(eventType, "Handler", stopwatch.ElapsedMilliseconds, endToEndLatency);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _metrics.RecordEventFailed(eventType, "Handler", ex.GetType().Name);
            throw;
        }
        finally
        {
            _metrics.DecrementActiveHandlers();
        }
    }
}
