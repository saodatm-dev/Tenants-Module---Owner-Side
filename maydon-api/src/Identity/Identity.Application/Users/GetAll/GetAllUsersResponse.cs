using Identity.Domain.Users;

namespace Identity.Application.Users.GetAll;

public sealed record GetAllUsersResponse(
	Guid Id,
	string? PhoneNumber,
	string FirstName,
	string LastName,
	string? MiddleName,
	string? Photo,
	int AccountsCount,
	bool IsVerified,
	RegisterType RegisterType,
	bool IsActive);
