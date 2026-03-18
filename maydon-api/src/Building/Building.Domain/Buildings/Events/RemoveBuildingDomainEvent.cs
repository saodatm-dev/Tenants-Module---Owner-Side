using Core.Domain.Events;

namespace Building.Domain.Buildings.Events;

public sealed record RemoveBuildingDomainEvent(Guid Id) : IPrePublishDomainEvent;
