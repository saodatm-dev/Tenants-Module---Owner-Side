namespace Building.Application.Dashboard;

public sealed record DashboardStatisticsResponse(
	RealEstateStats RealEstates,
	ListingStats Listings,
	ListingRequestStats ListingRequests,
	int LeasesCount,
	int WishlistsCount,
	decimal OccupancyRate);

public sealed record RealEstateStats(
	int Total,
	int Active,
	int Inactive,
	int Booked,
	int Rented);

public sealed record ListingStats(
	int Total,
	int Active);

public sealed record ListingRequestStats(
	int Total,
	int Pending);
