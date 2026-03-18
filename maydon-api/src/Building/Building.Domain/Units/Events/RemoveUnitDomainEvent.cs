using Core.Domain.Events;

namespace Building.Domain.Units.Events;

public sealed record RemoveUnitDomainEvent(Unit Unit) : IPrePublishDomainEvent;
