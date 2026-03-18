using Core.Application.Abstractions.Messaging;

namespace Identity.Application.Authentication.Registration.EImzoMobile;

public sealed record EImzoMobileRegistrationCommand(string DocumentId) : ICommand<AuthenticationResponse>;
