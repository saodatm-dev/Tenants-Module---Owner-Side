using Core.Domain.Events;
using Core.Domain.Languages;

namespace Building.Domain.Complexes.Events;

public sealed record CreateOrUpdateComplexDomainEvent(
	Guid Id,
	IEnumerable<LanguageValue> Descriptions,
	IEnumerable<string>? Images = null) : IPrePublishDomainEvent;
