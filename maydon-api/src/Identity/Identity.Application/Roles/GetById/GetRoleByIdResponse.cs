namespace Identity.Application.Roles.GetById;

public sealed record GetRoleByIdResponse(
	Guid Id,
	string Name,
	IEnumerable<GetRoleByIdPermissionsResponse> Permissions);


public sealed record GetRoleByIdPermissionsResponse(
	Guid Id,
	string Name,
	bool IsSelected);
