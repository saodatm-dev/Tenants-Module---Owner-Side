namespace Building.Application.Amenities.GetByRealEstateId;

public sealed record GetAmenitiesByRealEstateIdResponse(
	string Category,
	IEnumerable<GetAmenityResponse> Amenities);


public sealed record GetAmenityResponse(
	Guid Id,
	string Name,
	string IconUrl);
