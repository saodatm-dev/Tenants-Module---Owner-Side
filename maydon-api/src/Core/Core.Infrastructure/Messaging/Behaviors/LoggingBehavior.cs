using System.Diagnostics;
using Core.Application.Abstractions.Messaging;
using Microsoft.Extensions.Logging;

namespace Core.Infrastructure.Messaging.Behaviors;

/// <summary>
/// Logging decorator for integration event handlers.
/// Logs event processing with timing and error information.
/// </summary>
public sealed class LoggingBehavior(ILogger<LoggingBehavior> logger) : IGlobalIntegrationEventBehavior
{
    private readonly ILogger<LoggingBehavior> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task HandleAsync(IIntegrationEvent @event, Func<Task> next, CancellationToken cancellationToken)
    {
        var eventType = @event.GetType().Name;
        var eventId = @event.EventId;
        var stopwatch = Stopwatch.StartNew();

        _logger.LogInformation("Processing integration event {EventType} with ID {EventId}", eventType, eventId);

        try
        {
            await next();
            
            stopwatch.Stop();
            _logger.LogInformation("Successfully processed integration event {EventType} with ID {EventId} in {ElapsedMs}ms",
                eventType, eventId, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Failed to process integration event {EventType} with ID {EventId} after {ElapsedMs}ms", eventType, eventId, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
}
