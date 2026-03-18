using Core.Application.Abstractions.Messaging;
using Core.Domain.Roles;

namespace Identity.Application.Roles.Create;

public sealed record CreateRoleCommand(
	string Name,
	RoleType Type,
	IEnumerable<Guid> PermissionIds) : ICommand<Guid>;
