using Building.Domain.Floors;
using Core.Application.Abstractions.Messaging;

namespace Building.Application.Floors.Update;

public sealed record UpdateFloorCommand(
	Guid Id,
	short Number,
	FloorType? Type = null,
	string? Label = null,
	float? TotalArea = null,
	float? CeilingHeight = null,
	string? FloorPlan = null,
	Guid? BuildingId = null,
	Guid? RealEstateId = null) : ICommand<Guid>;
