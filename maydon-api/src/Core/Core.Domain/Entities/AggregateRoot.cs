using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Core.Domain.Events;

namespace Core.Domain.Entities;

/// <summary>
/// Base class for aggregate roots with generic key type.
/// Used by Document/Didox modules (e.g., DocumentEntity : AggregateRoot&lt;Guid&gt;).
/// </summary>
public abstract class AggregateRoot<TKey> : EntityBase<TKey>, IHasDomainEvents where TKey : notnull
{
    private readonly List<IDomainEvent> _domainEvents = [];

    [NotMapped]
    [JsonIgnore]
    public List<IDomainEvent> DomainEvents => [.. _domainEvents];

    public void ClearDomainEvents() => _domainEvents.Clear();

    public void Raise(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
}
