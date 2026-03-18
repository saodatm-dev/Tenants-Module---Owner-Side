using Core.Application.Abstractions.Messaging;

namespace Identity.Application.Authentication.Registration.OneId;

public sealed record OneIdRegistrationCommand(Guid Code) : ICommand<AuthenticationResponse>;
