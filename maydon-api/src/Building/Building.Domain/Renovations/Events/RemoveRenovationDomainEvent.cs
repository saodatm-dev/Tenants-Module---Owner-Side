using Core.Domain.Events;

namespace Building.Domain.Renovations.Events;

public sealed record RemoveRenovationDomainEvent(Guid Id) : IPrePublishDomainEvent;
