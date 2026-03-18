using Core.Domain.Events;

namespace Identity.Domain.Accounts.Events;

public sealed record DeleteAccountPreDomainEvent(Guid TenantId, Guid UserId) : IPrePublishDomainEvent;
