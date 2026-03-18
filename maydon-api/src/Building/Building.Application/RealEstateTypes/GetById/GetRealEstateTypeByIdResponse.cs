using Core.Domain.Languages;

namespace Building.Application.RealEstateTypes.GetById;

public sealed record GetRealEstateTypeByIdResponse(
	Guid Id,
	string TypeName,
	string? IconUrl,
	IEnumerable<LanguageValue> Names,
	IEnumerable<LanguageValue> Descriptions,
	bool ShowBuildingSuggestion,
	bool ShowFloorSuggestion,
	bool CanHaveUnits,
	bool CanHaveMeters,
	bool CanHaveFloors);
