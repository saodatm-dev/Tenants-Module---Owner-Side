using Core.Domain.Events;

namespace Identity.Domain.Users.Events;

public sealed record DeleteUserPreDomainEvent(Guid UserId) : IPrePublishDomainEvent;
