namespace Building.Application.RealEstates.Get;

public sealed record GetRealEstatesResponse(
	Guid Id,
	Guid OwnerId,
	string Type,
	string? Building,
	short? Floor,
	string? Number,
	int? RoomsCount,
	float? TotalArea,
	float? LivingArea,
	float? CeilingHeight,
	short? AboveFloors,
	short? BelowFloors,
	string? Region,
	string? District,
	double? Latitude,
	double? Longitude,
	string? Address,
	string? Plan,
    IEnumerable<string> Images);
