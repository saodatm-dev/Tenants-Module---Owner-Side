namespace Building.Application.Rooms.Get;

public sealed record GetRoomsResponse(
	Guid Id,
	Guid RealEstateId,
	string RoomType,
	float? Area);
