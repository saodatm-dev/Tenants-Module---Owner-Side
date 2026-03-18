using Core.Domain.Events;
using Core.Domain.Languages;

namespace Common.Domain.Banks.Events;

public sealed record CreateOrUpdateBankDomainEvent(
	Guid BankId,
	IEnumerable<LanguageValue> Translates) : IPrePublishDomainEvent;
