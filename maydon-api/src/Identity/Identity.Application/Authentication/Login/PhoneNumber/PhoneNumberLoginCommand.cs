using Core.Application.Abstractions.Messaging;

namespace Identity.Application.Authentication.Login.PhoneNumber;

public sealed record PhoneNumberLoginCommand(
	string PhoneNumber,
	string Password) : ICommand<AuthenticationResponse>;
