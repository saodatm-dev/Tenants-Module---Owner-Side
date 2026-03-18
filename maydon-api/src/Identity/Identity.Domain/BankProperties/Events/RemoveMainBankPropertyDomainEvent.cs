using Core.Domain.Events;

namespace Identity.Domain.BankProperties.Events;

public sealed record RemoveMainBankPropertyDomainEvent(
	Guid Id,
	Guid TenantId) : IPrePublishDomainEvent;
