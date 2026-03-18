using Building.Application.Floors.Get;

namespace Building.Application.Buildings.GetById;

public sealed record GetBuildingByIdResponse(
	Guid Id,
	Guid? ComplexId,
	Guid? RegionId,
	Guid? DistrictId,
	string Number,
	string Description,
	bool IsCommercial,
	bool IsLiving,
	double? Latitude,
	double? Longitude,
	string? Address,
	float? TotalArea,
	IEnumerable<string>? Images,
	IEnumerable<GetFloorsResponse> Floors);
