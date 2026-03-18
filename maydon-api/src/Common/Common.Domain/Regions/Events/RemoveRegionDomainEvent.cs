using Core.Domain.Events;

namespace Common.Domain.Regions.Events;

public sealed record RemoveRegionDomainEvent(Guid Id) : IPrePublishDomainEvent;
