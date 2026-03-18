using Core.Application.Abstractions.Messaging;

namespace Identity.Application.Authentication.Authorize.OneId;

public sealed record OneIdAuthCommand(Guid Code) : ICommand<AuthenticationResponse>;
