using Core.Application.Abstractions.Messaging;

namespace Identity.Application.Invitations.Cancel;

public sealed record CancelInvitationCommand(Guid Id) : ICommand;
