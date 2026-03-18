using Core.Domain.Languages;

namespace Common.Application.Currencies.GetById;

public sealed record GetCurrencyByIdResponse(
	Guid Id,
	string Code,
	IEnumerable<LanguageValue> Translates);
