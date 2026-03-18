using Core.Domain.Events;

namespace Building.Domain.LandCategories.Events;

public sealed record RemoveLandCategoryDomainEvent(Guid Id) : IPrePublishDomainEvent;
