using Core.Application.Abstractions.Messaging;

namespace Identity.Application.Authentication.RefreshToken;

public sealed record RefreshTokenCommand(string RefreshToken) : ICommand<AuthenticationResponse>;
