namespace Identity.Application.Users.GetPermissions;

public sealed record GetPermissionsResponse(
	Guid Id,
	string Module,
	string Group,
	string Name,
	bool IsActive);
