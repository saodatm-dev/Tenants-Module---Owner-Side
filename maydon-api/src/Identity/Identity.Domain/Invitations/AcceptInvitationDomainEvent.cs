using Core.Domain.Events;

namespace Identity.Domain.Invitations;

public sealed record AcceptInvitationDomainEvent(Guid CompanyId, Guid UserId, Guid RoleId) : IPrePublishDomainEvent;
