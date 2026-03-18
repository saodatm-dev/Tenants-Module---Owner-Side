using Core.Application.Abstractions.Messaging;
using Core.Domain.Languages;
using Identity.Domain.Otps;

namespace Identity.Application.OtpContents.Update;

public sealed record UpdateOtpContentCommand(
	OtpType OtpType,
	List<LanguageValue> Translates) : ICommand;
