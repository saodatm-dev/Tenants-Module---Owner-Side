using Core.Application.Abstractions.Messaging;

namespace Identity.Application.Invitations.Create;

public sealed record CreateInvitationCommand(
	string PhoneNumber,
	Guid RoleId) : ICommand<Guid>;
