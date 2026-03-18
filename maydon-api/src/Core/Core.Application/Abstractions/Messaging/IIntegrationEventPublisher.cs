namespace Core.Application.Abstractions.Messaging;

public interface IIntegrationEventPublisher
{
	Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
		where TEvent : class, IIntegrationEvent;
}
