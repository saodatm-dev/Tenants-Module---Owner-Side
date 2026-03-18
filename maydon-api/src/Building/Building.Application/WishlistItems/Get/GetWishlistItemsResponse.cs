using Building.Application.RealEstates.Get;

namespace Building.Application.WishlistItems.Get;

public sealed record GetWishlistItemsResponse(
	Guid Id,
	GetRealEstatesResponse RealEstate);
