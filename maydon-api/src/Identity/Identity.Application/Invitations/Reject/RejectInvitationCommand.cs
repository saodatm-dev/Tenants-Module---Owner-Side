using Core.Application.Abstractions.Messaging;

namespace Identity.Application.Invitations.Reject;

public sealed record RejectInvitationCommand(Guid Id, string? Reason) : ICommand;
