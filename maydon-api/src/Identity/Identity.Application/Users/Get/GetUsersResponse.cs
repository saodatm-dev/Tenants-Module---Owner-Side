namespace Identity.Application.Users.Get;

public sealed record GetUsersResponse(
	Guid Id,
	string FirstName,
	string LastName,
	string? MiddleName,
	string? Photo);
