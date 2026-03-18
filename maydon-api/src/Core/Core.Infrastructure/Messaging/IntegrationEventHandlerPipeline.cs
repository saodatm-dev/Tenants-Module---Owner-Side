using Core.Application.Abstractions.Messaging;
using Microsoft.Extensions.Logging;

namespace Core.Infrastructure.Messaging;

/// <summary>
/// Pipeline that wraps integration event handler invocation with cross-cutting behaviors.
/// Implements decorator pattern with ordered behavior execution.
/// </summary>
public sealed class IntegrationEventHandlerPipeline(
    IEnumerable<IGlobalIntegrationEventBehavior> behaviors,
    IServiceProvider serviceProvider,
    ILogger<IntegrationEventHandlerPipeline> logger) : IIntegrationEventHandlerPipeline
{
    private readonly List<IGlobalIntegrationEventBehavior> _behaviors = behaviors?.ToList() ?? [];
    private readonly IServiceProvider _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    private readonly ILogger<IntegrationEventHandlerPipeline> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task HandleAsync<TEvent>(TEvent @event, CancellationToken cancellationToken) 
        where TEvent : class, IIntegrationEvent
    {
        // Resolve handler from DI
        var handler = _serviceProvider.GetService(typeof(IIntegrationEventHandler<TEvent>)) as IIntegrationEventHandler<TEvent>;
        if (handler is null)
        {
            _logger.LogWarning("No handler registered for event type {EventType}", typeof(TEvent).Name);
            return;
        }

        // Build the pipeline from innermost (handler) to outermost (first behavior)
        // Execution order: Behavior[0] → Behavior[1] → ... → Handler
        Func<Task> pipeline = () => handler.Handle(@event, cancellationToken);

        // Wrap in reverse order so first registered behavior executes first
        for (var i = _behaviors.Count - 1; i >= 0; i--)
        {
            var behavior = _behaviors[i];
            var next = pipeline;
            pipeline = () => behavior.HandleAsync(@event, next, cancellationToken);
        }

        try
        {
            await pipeline();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Pipeline execution failed for {EventType} with handler {HandlerType}",
                typeof(TEvent).Name, handler.GetType().Name);
            throw;
        }
    }
}
