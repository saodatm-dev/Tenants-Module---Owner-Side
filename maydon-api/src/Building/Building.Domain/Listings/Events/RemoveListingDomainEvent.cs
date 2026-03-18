using Core.Domain.Events;

namespace Building.Domain.Listings.Events;

public sealed record RemoveListingDomainEvent(Guid ListingId) : IPrePublishDomainEvent;
