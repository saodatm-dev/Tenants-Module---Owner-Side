namespace Identity.Domain.Users;

public sealed record UserAccount(
	Guid 
	Guid? CompanyId,
	Guid UserId,
	Guid RoleId,
	bool IsHost,
	string UserName,
	string CompanyName,
	Guid? SessionId = null);
