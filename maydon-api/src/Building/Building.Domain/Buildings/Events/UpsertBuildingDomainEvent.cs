using Core.Domain.Events;
using Core.Domain.Languages;

namespace Building.Domain.Buildings.Events;

public sealed record UpsertBuildingDomainEvent(
	Guid BuildingId,
	IEnumerable<LanguageValue> Descriptions,
	IEnumerable<string>? Images = null) : IPrePublishDomainEvent;
