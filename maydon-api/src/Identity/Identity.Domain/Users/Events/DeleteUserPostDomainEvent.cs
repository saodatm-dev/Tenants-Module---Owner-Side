using Core.Domain.Events;

namespace Identity.Domain.Users.Events;

public sealed record DeleteUserPostDomainEvent(Guid UserId) : IPostPublishDomainEvent;
