using Core.Application.Abstractions.Messaging;

namespace Identity.Application.Users.UpdateProfile;

public sealed record UpdateProfileCommand(
	string? ProfilePicture,
	string? FirstName,
	string? LastName,
	string? MiddleName) : ICommand<Guid>;
