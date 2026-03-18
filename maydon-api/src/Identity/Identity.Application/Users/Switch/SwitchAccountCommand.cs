using Core.Application.Abstractions.Messaging;
using Identity.Application.Authentication;

namespace Identity.Application.Users.Switch;

public sealed class SwitchAccountCommand : ICommand<AuthenticationResponse>;
