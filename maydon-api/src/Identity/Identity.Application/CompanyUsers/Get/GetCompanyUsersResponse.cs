namespace Identity.Application.CompanyUsers.Get;

public sealed record GetCompanyUsersResponse(
	Guid UserId,
	string FullName,
	string PhoneNumber,
	string? Photo,
	string? RoleName,
	bool IsOwner,
	bool IsActive,
	DateTime JoinedAt);

