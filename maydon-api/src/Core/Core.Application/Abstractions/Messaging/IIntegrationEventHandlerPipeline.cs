namespace Core.Application.Abstractions.Messaging;

/// <summary>
/// Pipeline for processing integration events through a chain of behaviors.
/// </summary>
public interface IIntegrationEventHandlerPipeline
{
	Task HandleAsync<TEvent>(TEvent @event, CancellationToken cancellationToken)
		where TEvent : class, IIntegrationEvent;
}

/// <summary>
/// Marker interface for pipeline behaviors that apply to all events
/// </summary>
public interface IGlobalIntegrationEventBehavior
{
	Task HandleAsync(IIntegrationEvent @event, Func<Task> next, CancellationToken cancellationToken);
}
