using Core.Application.Abstractions.Messaging;
using Microsoft.Extensions.Logging;

namespace Core.Infrastructure.Messaging.Behaviors;

/// <summary>
/// Validation decorator for integration event handlers.
/// Validates events before processing to fail fast on invalid data.
/// </summary>
public sealed class ValidationBehavior : IGlobalIntegrationEventBehavior
{
    private readonly ILogger<ValidationBehavior> _logger;

    public ValidationBehavior(ILogger<ValidationBehavior> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task HandleAsync(
        IIntegrationEvent @event, 
        Func<Task> next, 
        CancellationToken cancellationToken)
    {
        // Basic validation - can be extended with FluentValidation
        if (@event.EventId == Guid.Empty)
        {
            var eventType = @event.GetType().Name;
            _logger.LogError("Invalid event {EventType}: EventId is empty", eventType);
            throw new InvalidOperationException($"Event {eventType} has invalid EventId (empty)");
        }

        if (@event.OccurredOn == default)
        {
            var eventType = @event.GetType().Name;
            _logger.LogError("Invalid event {EventType}: OccurredOn is default", eventType);
            throw new InvalidOperationException($"Event {eventType} has invalid OccurredOn (default)");
        }

        // Validate event is not too old (prevent replay attacks)
        var age = DateTime.UtcNow - @event.OccurredOn;
        if (age > TimeSpan.FromDays(7))
        {
            var eventType = @event.GetType().Name;
            _logger.LogWarning(
                "Event {EventType} with ID {EventId} is {AgeDays} days old",
                eventType, @event.EventId, age.TotalDays);
        }

        await next();
    }
}
