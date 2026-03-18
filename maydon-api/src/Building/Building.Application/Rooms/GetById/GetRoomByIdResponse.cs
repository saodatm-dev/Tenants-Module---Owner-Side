namespace Building.Application.Rooms.GetById;

public sealed record GetRoomByIdResponse(
	Guid Id,
	Guid RealEstateId,
	string RoomType,
	float? Area);
