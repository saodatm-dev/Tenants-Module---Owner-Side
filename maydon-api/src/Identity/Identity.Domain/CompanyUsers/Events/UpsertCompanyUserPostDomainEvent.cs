using Core.Domain.Events;

namespace Identity.Domain.CompanyUsers.Events;

public sealed record UpsertCompanyUserPostDomainEvent(CompanyUser CompanyUser) : IPostPublishDomainEvent;
