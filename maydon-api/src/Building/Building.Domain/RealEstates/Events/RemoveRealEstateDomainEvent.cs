using Core.Domain.Events;

namespace Building.Domain.RealEstates.Events;

public sealed record RemoveRealEstateDomainEvent(Guid RealEstateId) : IPrePublishDomainEvent;
