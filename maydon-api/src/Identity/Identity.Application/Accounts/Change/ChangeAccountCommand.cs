using Core.Application.Abstractions.Messaging;
using Identity.Application.Authentication;

namespace Identity.Application.Accounts.Change;

public sealed record ChangeAccountCommand(string Key) : ICommand<AuthenticationResponse>;
