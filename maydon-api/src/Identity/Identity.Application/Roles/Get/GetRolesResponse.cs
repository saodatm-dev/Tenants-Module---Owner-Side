namespace Identity.Application.Roles.Get;

public sealed record GetRolesResponse(
	Guid Id,
	string Name,
	bool IsSystem);
