using Core.Application.Abstractions.Messaging;

namespace Identity.Application.Users.Create;

public sealed record CreateUserCommand(
	string PhoneNumber,
	string Password,
	string Tin,
	string FirstName,
	string LastName,
	string? MiddleName = null) : ICommand<Guid>;
