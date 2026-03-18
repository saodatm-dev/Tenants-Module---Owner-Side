using Core.Application.Abstractions.Messaging;

namespace Identity.Application.Authentication.Registration.PhoneNumberConfirm;

public sealed record PhoneNumberRegistrationConfirmCommand(
	string Key,
	string Password) : ICommand<AuthenticationResponse>;
