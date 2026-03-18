using Building.Domain.Buildings;

namespace Building.Application.ListingCategories.Get;

public sealed record GetListingCategoriesResponse(
	Guid Id,
	Guid? ParentId,
	BuildingType BuildingType,
	string IconUrl,
	string Name);
