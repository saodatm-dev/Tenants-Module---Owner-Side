using Core.Application.Abstractions.Messaging;

namespace Building.Application.Rooms.Update;

public sealed record UpdateRoomCommand(
	Guid Id,
	Guid RealEstateId,
	Guid RoomTypeId,
	Guid? FloorId,
	string? Number,
	float TotalArea) : ICommand<Guid>;
