using Core.Domain.Events;

namespace Building.Domain.Complexes.Events;

public sealed record RemoveComplexDomainEvent(Guid Id) : IPrePublishDomainEvent;
