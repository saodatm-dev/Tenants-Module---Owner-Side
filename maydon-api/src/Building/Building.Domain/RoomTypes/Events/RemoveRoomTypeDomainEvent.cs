using Core.Domain.Events;

namespace Building.Domain.RoomTypes.Events;

public sealed record RemoveRoomTypeDomainEvent(Guid Id) : IPrePublishDomainEvent;
