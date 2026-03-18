using Core.Domain.Events;
using Core.Domain.Languages;

namespace Building.Domain.Listings.Events;

public sealed record UpsertListingDomainEvent(
	Listing Listing,
	IEnumerable<Guid>? AmenityIds = null,
	IEnumerable<LanguageValue>? DescriptionTranslates = null) : IPrePublishDomainEvent;
