using Building.Domain.Buildings;
using Building.Domain.Floors;

namespace Building.Application.Floors.Get;

public sealed record GetFloorsResponse(
	Guid Id,
	string? BuildingNumber,
	IEnumerable<BuildingType>? BuildingTypes,
	short Number,
	FloorType? Type,
	string? Label,
	float? TotalArea,
	float? CeilingHeight,
	string? Plan);
