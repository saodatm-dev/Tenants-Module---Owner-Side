using Core.Domain.Events;
using Core.Domain.Languages;

namespace Common.Domain.Currencies.Events;

public sealed record UpsertCurrencyDomainEvent(
	Guid Id,
	IEnumerable<LanguageValue> Translates) : IPrePublishDomainEvent;
