using Building.Domain.Buildings;
using Core.Application.Abstractions.Messaging;
using Core.Domain.Languages;

namespace Building.Application.ListingCategories.Create;

public sealed record CreateListingCategoryCommand(
	BuildingType BuildingType,
	string IconUrl,
	List<LanguageValue> Translates,
	bool ShowInMain = false,
	Guid? ParentId = null) : ICommand<Guid>;
