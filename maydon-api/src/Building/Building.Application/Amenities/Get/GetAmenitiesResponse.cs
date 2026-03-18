namespace Building.Application.Amenities.Get;

public sealed record GetAmenitiesResponse(
	Guid Id,
	string Name,
	string IconUrl);
