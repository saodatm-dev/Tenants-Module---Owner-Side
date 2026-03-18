using Building.Domain.Buildings;
using Core.Application.Abstractions.Messaging;
using Core.Domain.Languages;

namespace Building.Application.ListingCategories.Update;

public sealed record UpdateListingCategoryCommand(
	Guid Id,
	Guid? ParentId,
	BuildingType BuildingType,
	string IconUrl,
	List<LanguageValue> Translates,
	bool ShowInMain = false) : ICommand<Guid>;
