using Core.Application.Abstractions.Messaging;
using Core.Domain.Languages;

namespace Building.Application.RealEstateTypes.Create;

public sealed record CreateRealEstateTypeCommand(
	string TypeName,
	IEnumerable<LanguageValue> Names,
	IEnumerable<LanguageValue> Descriptions,
	string? IconUrl = null,
	bool ShowBuildingSuggestion = false,
	bool ShowFloorSuggestion = false,
	bool CanHaveUnits = false,
	bool CanHaveMeters = false,
	bool CanHaveFloors = false) : ICommand<Guid>;
