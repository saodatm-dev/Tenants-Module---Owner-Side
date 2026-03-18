using Core.Domain.Events;

namespace Identity.Domain.Invitations.Events;

public sealed record CreateInvitationDomainEvent(Invitation Invitation) : IPostPublishDomainEvent;
