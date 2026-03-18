using Core.Application.Abstractions.Messaging;
using Identity.Application.Authentication;

namespace Identity.Application.Accounts.CreateOwner;

public sealed record CreateOwnerAccountCommand : ICommand<AuthenticationResponse>;
