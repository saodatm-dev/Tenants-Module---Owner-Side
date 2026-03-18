using Core.Domain.Events;
using Core.Domain.Languages;

namespace Building.Domain.RentalPurposes.Events;

public sealed record UpsertRentalPurposeDomainEvent(
	Guid RentalPurposeId,
	IEnumerable<LanguageValue> Translates) : IPrePublishDomainEvent;
