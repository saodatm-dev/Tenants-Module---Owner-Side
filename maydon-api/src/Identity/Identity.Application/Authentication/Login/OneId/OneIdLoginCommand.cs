using Core.Application.Abstractions.Messaging;

namespace Identity.Application.Authentication.Login.OneId;

public sealed record OneIdLoginCommand(Guid Code) : ICommand<AuthenticationResponse>;
