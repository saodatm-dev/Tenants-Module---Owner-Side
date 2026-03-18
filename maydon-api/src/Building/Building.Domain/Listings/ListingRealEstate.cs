using Building.Domain.RealEstates;

namespace Building.Domain.Listings;

public sealed record ListingRealEstate(
	Guid RealEstateId,
	IEnumerable<RealEstatePlanCoordinate>? Coordinates);
