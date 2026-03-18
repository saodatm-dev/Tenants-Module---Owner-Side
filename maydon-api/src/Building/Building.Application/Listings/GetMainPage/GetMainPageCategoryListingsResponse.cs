namespace Building.Application.Listings.GetMainPage;

public sealed record GetMainPageCategoryListingsResponse(
	Guid Id,
	Guid OwnerId,
	string? Title,
	List<Guid> CategoryIds,
	string Image,
	string? Building,
	string? Complex,
	float? TotalArea,
	int? FloorsCount,
	string Description,
	long? PriceForMonth,
	long? PricePerSquareMeter,
	string? Region,
	string? District,
	double? Latitude,
	double? Longitude,
	string? Address);
