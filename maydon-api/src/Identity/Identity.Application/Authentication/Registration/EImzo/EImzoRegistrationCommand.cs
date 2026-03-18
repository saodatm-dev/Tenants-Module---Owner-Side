using Core.Application.Abstractions.Messaging;

namespace Identity.Application.Authentication.Registration.EImzo;

public sealed record EImzoRegistrationCommand(string Pkcs7) : ICommand<AuthenticationResponse>;
