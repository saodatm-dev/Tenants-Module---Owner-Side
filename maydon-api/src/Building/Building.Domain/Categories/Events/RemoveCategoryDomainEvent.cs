using Core.Domain.Events;

namespace Building.Domain.Categories.Events;

public sealed record RemoveCategoryDomainEvent(Guid Id) : IPrePublishDomainEvent;
