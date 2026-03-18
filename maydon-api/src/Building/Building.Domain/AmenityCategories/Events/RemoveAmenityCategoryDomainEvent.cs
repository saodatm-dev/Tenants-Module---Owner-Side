using Core.Domain.Events;

namespace Building.Domain.AmenityCategories.Events;

public sealed record RemoveAmenityCategoryDomainEvent(Guid Id) : IPrePublishDomainEvent;
