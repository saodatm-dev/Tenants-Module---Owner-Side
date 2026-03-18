namespace Building.Application.AmenityCategories.GetWithAmenities;

public sealed record GetAmenityCategoriesWithAmenitiesResponse(
	Guid Id,
	string Name,
	IEnumerable<AmenityItemResponse> Amenities);

public sealed record AmenityItemResponse(
	Guid Id,
	string Name,
	string IconUrl);
