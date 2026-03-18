using Core.Application.Abstractions.Messaging;

namespace Identity.Application.Invitations.Accept;

public sealed record AcceptInvitationCommand(Guid Id) : ICommand;
