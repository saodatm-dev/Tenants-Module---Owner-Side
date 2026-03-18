using Core.Application.Abstractions.Messaging;

namespace Identity.Application.Authentication.Registration.PhoneNumber;

public sealed record PhoneNumberRegistrationCommand(
	string PhoneNumber,
	string Code) : ICommand<string>;
