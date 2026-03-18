using Core.Application.Abstractions.Messaging;

namespace Identity.Application.Roles.Update;

public sealed record UpdateRoleCommand(
	Guid Id,
	string Name,
	IEnumerable<Guid> PermissionIds) : ICommand<Guid>;
