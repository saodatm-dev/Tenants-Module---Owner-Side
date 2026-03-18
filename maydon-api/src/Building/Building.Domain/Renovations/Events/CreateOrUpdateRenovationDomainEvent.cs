using Core.Domain.Events;
using Core.Domain.Languages;

namespace Building.Domain.Renovations.Events;

public sealed record CreateOrUpdateRenovationDomainEvent(
	Guid RenovationId,
	IEnumerable<LanguageValue> Translates) : IPrePublishDomainEvent;
