using Core.Application.Abstractions.Messaging;

namespace Identity.Application.Roles.Clone;

public sealed record CloneRoleCommand(
	Guid RoleId,
	string Name) : ICommand<Guid>;
