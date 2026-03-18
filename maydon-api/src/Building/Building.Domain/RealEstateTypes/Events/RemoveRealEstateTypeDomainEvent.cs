using Core.Domain.Events;

namespace Building.Domain.RealEstateTypes.Events;

public sealed record RemoveRealEstateTypeDomainEvent(RealEstateType RealEstateType) : IPrePublishDomainEvent;
