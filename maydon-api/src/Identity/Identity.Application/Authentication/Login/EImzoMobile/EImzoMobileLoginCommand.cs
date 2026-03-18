using Core.Application.Abstractions.Messaging;

namespace Identity.Application.Authentication.Login.EImzoMobile;

public sealed record EImzoMobileLoginCommand(string DocumentId) : ICommand<AuthenticationResponse>;
