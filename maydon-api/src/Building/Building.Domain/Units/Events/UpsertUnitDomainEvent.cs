using Building.Domain.Floors;
using Building.Domain.Rooms;
using Core.Domain.Events;

namespace Building.Domain.Units.Events;

public sealed record UpsertUnitDomainEvent(
	Unit Unit,
	IEnumerable<RoomValue>? Rooms = null) : IPrePublishDomainEvent;
