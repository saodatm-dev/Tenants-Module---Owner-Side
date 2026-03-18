using Core.Domain.Events;
using Core.Domain.Languages;

namespace Building.Domain.ListingCategories.Events;

public sealed record UpsertListingCategoryDomainEvent(
	Guid ListingCategoryId,
	IEnumerable<LanguageValue> Translates) : IPrePublishDomainEvent;
