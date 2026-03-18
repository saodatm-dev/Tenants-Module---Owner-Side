using Core.Domain.Events;

namespace Building.Domain.Amenities.Events;

public sealed record RemoveAmenityDomainEvent(Guid Id) : IPrePublishDomainEvent;
