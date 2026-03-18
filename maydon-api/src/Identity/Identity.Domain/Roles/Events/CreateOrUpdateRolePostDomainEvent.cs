using Core.Domain.Events;

namespace Identity.Domain.Roles.Events;

public sealed record CreateOrUpdateRolePostDomainEvent(Role Role) : IPostPublishDomainEvent;
