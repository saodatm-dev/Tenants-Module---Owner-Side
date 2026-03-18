using Core.Domain.Events;

namespace Building.Domain.RentalPurposes.Events;

public sealed record RemoveRentalPurposeDomainEvent(Guid Id) : IPrePublishDomainEvent;
