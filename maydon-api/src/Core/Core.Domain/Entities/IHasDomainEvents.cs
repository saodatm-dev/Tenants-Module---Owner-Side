using Core.Domain.Events;

namespace Core.Domain.Entities;

/// <summary>
/// Interface for entities that can raise domain events.
/// Implemented by both <see cref="Entity"/> and <see cref="AggregateRoot{TKey}"/>.
/// </summary>
public interface IHasDomainEvents
{
    List<IDomainEvent> DomainEvents { get; }
    void ClearDomainEvents();
    void Raise(IDomainEvent domainEvent);
}
