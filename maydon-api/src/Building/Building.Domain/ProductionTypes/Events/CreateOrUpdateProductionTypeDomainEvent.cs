using Core.Domain.Events;
using Core.Domain.Languages;

namespace Building.Domain.ProductionTypes.Events;

public sealed record CreateOrUpdateProductionTypeDomainEvent(
	Guid ProductionTypeId,
	IEnumerable<LanguageValue> Translates) : IPrePublishDomainEvent;
