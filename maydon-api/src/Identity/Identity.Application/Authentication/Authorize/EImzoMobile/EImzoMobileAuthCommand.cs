using Core.Application.Abstractions.Messaging;

namespace Identity.Application.Authentication.Authorize.EImzoMobile;

public sealed record EImzoMobileAuthCommand(string DocumentId) : ICommand<AuthenticationResponse>;
