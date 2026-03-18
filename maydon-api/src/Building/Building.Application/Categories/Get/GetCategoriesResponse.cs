using Building.Domain.Buildings;

namespace Building.Application.Categories.Get;

public sealed record GetCategoriesResponse(
	Guid Id,
	BuildingType BuildingType,
	string IconUrl,
	string Name);
