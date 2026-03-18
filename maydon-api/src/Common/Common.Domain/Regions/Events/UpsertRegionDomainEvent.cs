using Core.Domain.Events;
using Core.Domain.Languages;

namespace Common.Domain.Regions.Events;

public sealed record UpsertRegionDomainEvent(
	Guid RegionId,
	IEnumerable<LanguageValue> Translates) : IPrePublishDomainEvent;
