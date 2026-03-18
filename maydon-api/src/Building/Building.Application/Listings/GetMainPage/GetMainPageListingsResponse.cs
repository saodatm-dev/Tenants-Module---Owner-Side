namespace Building.Application.Listings.GetMainPage;

public sealed record GetMainPageListingsResponse(
	Guid ListingCategoryId,
	string ListingCategoryName,
	IEnumerable<GetMainPageCategoryListingsResponse> Listings);
