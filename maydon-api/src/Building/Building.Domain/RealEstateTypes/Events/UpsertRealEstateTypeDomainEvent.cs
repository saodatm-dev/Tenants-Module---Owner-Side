using Core.Domain.Events;
using Core.Domain.Languages;

namespace Building.Domain.RealEstateTypes.Events;

public sealed record UpsertRealEstateTypeDomainEvent(
	Guid RealEstateTypeId,
	IEnumerable<LanguageValue> Names,
	IEnumerable<LanguageValue> Descriptions) : IPrePublishDomainEvent;
