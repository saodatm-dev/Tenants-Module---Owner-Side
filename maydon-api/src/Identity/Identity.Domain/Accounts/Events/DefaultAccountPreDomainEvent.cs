using Core.Domain.Events;

namespace Identity.Domain.Accounts.Events;

public sealed record DefaultAccountPreDomainEvent(Account Account) : IPrePublishDomainEvent;
