using Core.Domain.Events;

namespace Identity.Domain.Users.Events;

public sealed record UpsertUserPreDomainEvent(User User) : IPrePublishDomainEvent;
