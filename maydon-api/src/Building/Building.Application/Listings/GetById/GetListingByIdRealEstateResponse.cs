namespace Building.Application.Listings.GetById;

public sealed record GetListingByIdRealEstateResponse(
	Guid Id,
	string? Number,
	float? TotalArea,
	int? RoomsCount = null,
	float? LivingArea = null,
	float? CeilingHeight = null,
	string? Plan = null,
	IEnumerable<GetListingByIdRoomTypeResponse>? Rooms = null);
