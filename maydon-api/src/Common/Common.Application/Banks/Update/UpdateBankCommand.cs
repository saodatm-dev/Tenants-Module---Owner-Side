using Core.Application.Abstractions.Messaging;
using Core.Domain.Languages;

namespace Common.Application.Banks.Update;

public sealed record UpdateBankCommand(
	Guid Id,
	string Mfo,
	string? Tin,
	string? PhoneNumber,
	string? Email,
	string? Website,
	string Address,
	List<LanguageValue> Translates) : ICommand<Guid>;
