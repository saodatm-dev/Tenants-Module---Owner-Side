using Core.Domain.Events;
using Core.Domain.Languages;

namespace Building.Domain.Amenities.Events;

public sealed record UpsertAmenityDomainEvent(
	Guid AmenityId,
	IEnumerable<LanguageValue> Translates) : IPrePublishDomainEvent;
