using Core.Application.Abstractions.Messaging;

namespace Identity.Application.Roles.Remove;

public sealed record RemoveRoleCommand(Guid Id) : ICommand;
