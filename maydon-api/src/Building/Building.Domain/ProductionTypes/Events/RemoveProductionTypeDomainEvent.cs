using Core.Domain.Events;

namespace Building.Domain.ProductionTypes.Events;

public sealed record RemoveProductionTypeDomainEvent(Guid Id) : IPrePublishDomainEvent;
