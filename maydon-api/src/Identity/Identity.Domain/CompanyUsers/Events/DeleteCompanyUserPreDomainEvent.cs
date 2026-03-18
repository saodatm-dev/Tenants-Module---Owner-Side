using Core.Domain.Events;

namespace Identity.Domain.CompanyUsers.Events;

public sealed record DeleteCompanyUserPreDomainEvent(Guid CompanyUserId) : IPrePublishDomainEvent;
