using Core.Domain.Languages;

namespace Common.Application.Banks.GetById;

public sealed record GetBankByIdResponse(
	Guid Id,
	string Mfo,
	string? Tin,
	string? PhoneNumber,
	string? Email,
	string? Website,
	string? Address,
	IEnumerable<LanguageValue> Translates);
