using Core.Application.Abstractions.Messaging;

namespace Identity.Application.Authentication.ForgotPassword.PhoneNumberConfirm;

public sealed record PhoneNumberForgotPasswordConfirmCommand(
	string Key,
	string Password) : ICommand<AuthenticationResponse>;
