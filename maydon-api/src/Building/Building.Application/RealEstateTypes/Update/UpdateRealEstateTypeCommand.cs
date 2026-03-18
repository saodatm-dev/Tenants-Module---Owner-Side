using Core.Application.Abstractions.Messaging;
using Core.Domain.Languages;

namespace Building.Application.RealEstateTypes.Update;

public sealed record UpdateRealEstateTypeCommand(
	Guid Id,
	string TypeName,
	IEnumerable<LanguageValue> Names,
	IEnumerable<LanguageValue> Descriptions,
	string? IconUrl = null,
	bool ShowBuildingSuggestion = false,
	bool ShowFloorSuggestion = false,
	bool CanHaveUnits = false,
	bool CanHaveMeters = false,
	bool CanHaveFloors = false) : ICommand<Guid>;
