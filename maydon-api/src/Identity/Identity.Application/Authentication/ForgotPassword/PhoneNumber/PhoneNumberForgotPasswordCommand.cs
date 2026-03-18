using Core.Application.Abstractions.Messaging;

namespace Identity.Application.Authentication.ForgotPassword.PhoneNumber;

public sealed record PhoneNumberForgotPasswordCommand(
	string PhoneNumber,
	string Code) : ICommand<string>;
