using Building.Domain.Buildings;
using Core.Domain.Languages;

namespace Building.Application.ListingCategories.GetById;

public sealed record GetListingCategoriesByIdResponse(
	Guid Id,
	Guid? ParentId,
	BuildingType BuildingType,
	string IconUrl,
	IEnumerable<LanguageValue> Translates);
