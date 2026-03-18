using Core.Domain.Events;
using Core.Domain.Languages;

namespace Building.Domain.AmenityCategories.Events;

public sealed record UpsertAmenityCategoryDomainEvent(
	Guid AmenityCategoryId,
	IEnumerable<LanguageValue> Translates) : IPrePublishDomainEvent;
