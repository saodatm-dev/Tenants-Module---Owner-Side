using Building.Domain.Listings;
using Building.Domain.Statuses;

namespace Building.Application.Listings.GetById;

public sealed record GetListingByIdResponse(
	Guid Id,
	Guid OwnerId,
	string? Title,
	IEnumerable<string> Categories,
	string? Complex,
	string? Building,
	IEnumerable<GetListingByIdFloorResponse>? Floors,
	int? RoomsCount,
	string? Description,
	IEnumerable<GetListingByIdAmenityResponse>? Amenities,
	string? Plan,
	float TotalArea,
	float? LivingArea,
	float? CeilingHeight,
	long? PriceForMonth,
	long? PricePerSquareMeter,
	string? Region,
	string? District,
	double? Latitude,
	double? Longitude,
	string? Address,
    Status Status,
    IEnumerable<string>? Images,
	string? RentalPurpose,
	MinLeaseTerm? MinLeaseTerm,
	UtilityPaymentType? UtilityPaymentType,
	DateOnly? NextAvailableDate);
