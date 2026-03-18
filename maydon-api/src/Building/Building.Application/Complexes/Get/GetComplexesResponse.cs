namespace Building.Application.Complexes.Get;

public sealed record GetComplexesResponse(
	Guid Id,
	string Region,
	string District,
	string Name,
	string? Description,
	bool IsCommercial,
	bool IsLiving,
	double? Latitude,
	double? Longitude,
	string? Address,
	IEnumerable<string>? Images);
