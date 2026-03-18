using Core.Application.Abstractions.Messaging;

namespace Building.Application.Rooms.Create;

public sealed record CreateRoomCommand(
	Guid RealEstateId,
	Guid RoomTypeId,
	Guid? FloorId,
	string? Number,
	float TotalArea) : ICommand<Guid>;
