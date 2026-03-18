using Core.Domain.Events;

namespace Building.Domain.MeterTypes.Events;

public sealed record RemoveMeterTypeDomainEvent(Guid Id) : IPrePublishDomainEvent;
