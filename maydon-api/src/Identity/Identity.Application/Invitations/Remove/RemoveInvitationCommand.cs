using Core.Application.Abstractions.Messaging;

namespace Identity.Application.Invitations.Remove;

public sealed record RemoveInvitationCommand(Guid Id) : ICommand;
