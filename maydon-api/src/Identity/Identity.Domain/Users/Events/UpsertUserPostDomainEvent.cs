using Core.Domain.Events;

namespace Identity.Domain.Users.Events;

public sealed record UpsertUserPostDomainEvent(User User) : IPostPublishDomainEvent;
