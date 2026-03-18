using Core.Application.Abstractions.Messaging;
using Identity.Application.Authentication;

namespace Identity.Application.Accounts.CreateClient;

public sealed record CreateClientAccountCommand : ICommand<AuthenticationResponse>;
