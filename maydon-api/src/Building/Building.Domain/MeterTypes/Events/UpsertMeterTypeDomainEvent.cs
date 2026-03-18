using Core.Domain.Events;
using Core.Domain.Languages;

namespace Building.Domain.MeterTypes.Events;

public sealed record UpsertMeterTypeDomainEvent(
	Guid MeterTypeId,
	IEnumerable<LanguageValue> Names,
	IEnumerable<LanguageValue> Units) : IPrePublishDomainEvent;
