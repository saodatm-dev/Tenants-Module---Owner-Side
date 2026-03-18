using Core.Domain.Events;
using Core.Domain.Languages;

namespace Common.Domain.Districts.Events;

public sealed record UpsertDistrictDomainEvent(
	Guid DistrictId,
	IEnumerable<LanguageValue> Translates) : IPrePublishDomainEvent;
