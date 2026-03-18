using Core.Domain.Events;

namespace Identity.Domain.Companies.Events;

public sealed record CreateOrUpdateCompanyPostDomainEvent(Company Company) : IPostPublishDomainEvent;
