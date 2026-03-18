using Core.Domain.Events;

namespace Identity.Domain.Accounts.Events;

public sealed record CreateAccountPreDomainEvent(
	Guid TenantId,
	Guid UserId,
	Guid RoleId,
	AccountType Type) : IPrePublishDomainEvent;
