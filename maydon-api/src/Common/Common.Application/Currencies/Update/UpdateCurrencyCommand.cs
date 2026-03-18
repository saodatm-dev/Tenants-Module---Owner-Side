using Core.Application.Abstractions.Messaging;
using Core.Domain.Languages;

namespace Common.Application.Currencies.Update;

public sealed record UpdateCurrencyCommand(
	Guid Id,
	string Code,
	List<LanguageValue> Translates) : ICommand<Guid>;
