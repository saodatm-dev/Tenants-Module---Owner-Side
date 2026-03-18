using Core.Domain.Events;

namespace Identity.Domain.CompanyUsers.Events;

public sealed record DeleteCompanyUserPostDomainEvent(Guid CompanyUserId) : IPostPublishDomainEvent;
