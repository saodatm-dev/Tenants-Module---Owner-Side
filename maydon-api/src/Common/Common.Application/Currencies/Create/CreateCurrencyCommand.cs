using Core.Application.Abstractions.Messaging;
using Core.Domain.Languages;

namespace Common.Application.Currencies.Create;

public sealed record CreateCurrencyCommand(
	string Code,
	List<LanguageValue> Translates) : ICommand<Guid>;
