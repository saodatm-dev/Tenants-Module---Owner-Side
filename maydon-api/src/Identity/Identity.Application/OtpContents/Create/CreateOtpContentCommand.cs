using Core.Application.Abstractions.Messaging;
using Core.Domain.Languages;
using Identity.Domain.Otps;

namespace Identity.Application.OtpContents.Create;

public sealed record CreateOtpContentCommand(
	OtpType OtpType,
	List<LanguageValue> Translates) : ICommand;
