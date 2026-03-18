using Core.Domain.Events;

namespace Identity.Domain.Companies.Events;

public sealed record DeleteCompanyPostDomainEvent(Guid CompanyId) : IPostPublishDomainEvent;
