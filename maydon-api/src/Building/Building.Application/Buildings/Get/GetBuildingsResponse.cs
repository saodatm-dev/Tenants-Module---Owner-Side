using Building.Application.Floors.Get;

namespace Building.Application.Buildings.Get;

public sealed record GetBuildingsResponse(
	Guid Id,
	string? Complex,
	string Region,
	string District,
	string Number,
	string Description,
	bool IsCommercial,
	bool IsLiving,
	double? Latitude,
	double? Longitude,
	string? Address,
	short? TotalArea,
	IEnumerable<string>? Images,
	IEnumerable<GetFloorsResponse> Floors);
