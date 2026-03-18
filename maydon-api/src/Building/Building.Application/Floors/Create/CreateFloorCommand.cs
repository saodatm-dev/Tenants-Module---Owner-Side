using Building.Domain.Floors;
using Core.Application.Abstractions.Messaging;

namespace Building.Application.Floors.Create;

public sealed record CreateFloorCommand(
	short Number,
	FloorType? Type = null,
	string? Label = null,
	float? TotalArea = null,
	float? CeilingHeight = null,
	string? Plan = null,
	Guid? BuildingId = null,
	Guid? RealEstateId = null) : ICommand<Guid>;
