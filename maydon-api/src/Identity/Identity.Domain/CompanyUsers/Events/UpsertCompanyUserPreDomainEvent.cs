using Core.Domain.Events;

namespace Identity.Domain.CompanyUsers.Events;

public sealed record UpsertCompanyUserPreDomainEvent(CompanyUser CompanyUser) : IPrePublishDomainEvent;
