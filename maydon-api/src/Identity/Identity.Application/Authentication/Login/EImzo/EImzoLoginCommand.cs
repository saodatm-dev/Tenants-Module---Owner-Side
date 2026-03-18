using Core.Application.Abstractions.Messaging;

namespace Identity.Application.Authentication.Login.EImzo;

public sealed record EImzoLoginCommand(string Pkcs7) : ICommand<AuthenticationResponse>;
