namespace Building.Application.RealEstateTypes.Get;

public sealed record GetRealEstateTypesResponse(
	Guid Id,
	string TypeName,
	string? Icon,
	string Name,
	string Description,
	bool ShowBuildingSuggestion = false,
	bool ShowFloorSuggestion = false,
	bool CanHaveUnits = false,
	bool CanHaveMeters = false,
	bool CanHaveFloors = false);
