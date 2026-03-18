namespace Core.Domain.Events;

public interface IDomainEvent
{
	Guid EventId => Guid.NewGuid();
	DateTime OccurredOn => DateTime.UtcNow;
}
