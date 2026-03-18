namespace Building.Domain.Rooms;

public sealed record RoomValue(
	Guid RoomTypeId,
	float? Area = null);
