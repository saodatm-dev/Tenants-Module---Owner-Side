using Core.Domain.Events;

namespace Core.Infrastructure.DomainEvents;

public interface IDomainEventPublisher
{
	Task PublishAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
}
