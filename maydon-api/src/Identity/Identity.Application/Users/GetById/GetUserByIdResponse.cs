namespace Identity.Application.Users.GetById;

public sealed record GetUserByIdResponse(
	Guid UserId,
	string FirstName,
	string LastName,
	string? MiddleName,
	string? PhoneNumber,
	string? Photo);
