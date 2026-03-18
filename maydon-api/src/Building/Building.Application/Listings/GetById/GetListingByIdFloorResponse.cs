using Building.Domain.Floors;

namespace Building.Application.Listings.GetById;

public sealed record GetListingByIdFloorResponse(
	Guid Id,
	short Number,
	FloorType? Type,
	string? Label,
	float? TotalArea,
	float? CeilingHeight = null,
	string? Plan = null);
