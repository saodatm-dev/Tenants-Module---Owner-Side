namespace Core.Domain.Events;

public interface IDomainEventHandler<in T> where T : IDomainEvent
{
	ValueTask Handle(T domainEvent, CancellationToken cancellationToken);
}
