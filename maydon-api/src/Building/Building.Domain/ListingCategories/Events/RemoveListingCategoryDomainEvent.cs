using Core.Domain.Events;

namespace Building.Domain.ListingCategories.Events;

public sealed record RemoveListingCategoryDomainEvent(Guid Id) : IPrePublishDomainEvent;
