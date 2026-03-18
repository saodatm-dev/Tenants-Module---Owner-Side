using Core.Domain.Events;

namespace Identity.Domain.Roles.Events;

public sealed record DeleteRolePostDomainEvent(Guid RoleId) : IPostPublishDomainEvent;
