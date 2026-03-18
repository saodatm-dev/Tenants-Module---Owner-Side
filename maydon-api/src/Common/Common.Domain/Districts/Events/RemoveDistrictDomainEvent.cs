using Core.Domain.Events;

namespace Common.Domain.Districts.Events;

public sealed record RemoveDistrictDomainEvent(Guid Id) : IPrePublishDomainEvent;
