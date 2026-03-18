namespace Building.Application.Complexes.GetById;

public sealed record GetComplexByIdResponse(
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
