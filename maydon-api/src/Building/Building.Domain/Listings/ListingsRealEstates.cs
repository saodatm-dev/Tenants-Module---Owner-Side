using Building.Domain.RealEstates;

namespace Building.Domain.Listings;

public sealed record ListingsRealEstates(
	RealEstate RealEstate,
	IEnumerable<RealEstatePlanCoordinate>? Coordinates);
