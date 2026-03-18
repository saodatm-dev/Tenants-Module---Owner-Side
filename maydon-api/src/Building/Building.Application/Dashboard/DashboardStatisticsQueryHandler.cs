using Building.Application.Core.Abstractions.Data;
using Building.Domain.ListingRequests;
using Building.Domain.Statuses;
using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Dashboard;

internal sealed class DashboardStatisticsQueryHandler(
	IExecutionContextProvider executionContextProvider,
	IBuildingDbContext dbContext) : IQueryHandler<DashboardStatisticsQuery, DashboardStatisticsResponse>
{
	public async Task<Result<DashboardStatisticsResponse>> Handle(
		DashboardStatisticsQuery query, 
		CancellationToken cancellationToken)
	{
		var ownerId = executionContextProvider.TenantId;

		var realEstateStats = await dbContext.RealEstates
			.Where(r => r.OwnerId == ownerId)
			.GroupBy(r => r.Status)
			.Select(g => new { Status = g.Key, Count = g.Count() })
			.ToListAsync(cancellationToken);
		
		var realEstateTotal = realEstateStats.Sum(s => s.Count);
		var realEstateActive = realEstateStats.FirstOrDefault(s => s.Status == Status.Active)?.Count ?? 0;
		var realEstateInactive = realEstateStats.FirstOrDefault(s => s.Status == Status.Inactive)?.Count ?? 0;
		var realEstateBooked = realEstateStats.FirstOrDefault(s => s.Status == Status.Booked)?.Count ?? 0;
		var realEstateRented = realEstateStats.FirstOrDefault(s => s.Status == Status.Rented)?.Count ?? 0;

		var listingStats = await dbContext.Listings
			.Where(l => l.OwnerId == ownerId)
			.GroupBy(l => l.Status)
			.Select(g => new { Status = g.Key, Count = g.Count() })
			.ToListAsync(cancellationToken);

		var listingsTotal = listingStats.Sum(s => s.Count);
		var listingsActive = listingStats.FirstOrDefault(s => s.Status == Status.Active)?.Count ?? 0;

		var listingRequestStats = await dbContext.ListingRequests
			.Where(lr => lr.OwnerId == ownerId)
			.GroupBy(lr => lr.Status)
			.Select(g => new { Status = g.Key, Count = g.Count() })
			.ToListAsync(cancellationToken);

		var listingRequestsTotal = listingRequestStats.Sum(s => s.Count);
		var listingRequestsPending = listingRequestStats
			.Where(s => s.Status == ListingRequestStatus.Sent || s.Status == ListingRequestStatus.Received)
			.Sum(s => s.Count);

		var leasesCount = await dbContext.Leases
			.CountAsync(l => l.OwnerId == ownerId, cancellationToken);

		var wishlistsCount = await dbContext.Wishlists
			.Join(dbContext.Listings,
				w => w.ListringId,
				l => l.Id,
				(w, l) => new { Wishlist = w, Listing = l })
			.CountAsync(wl => wl.Listing.OwnerId == ownerId, cancellationToken);

		var occupancyRate = realEstateTotal > 0
			? Math.Round((decimal)(realEstateBooked + realEstateRented) / realEstateTotal, 2)
			: 0;

		var response = new DashboardStatisticsResponse(
			RealEstates: new RealEstateStats(
				Total: realEstateTotal,
				Active: realEstateActive,
				Inactive: realEstateInactive,
				Booked: realEstateBooked,
				Rented: realEstateRented),
			Listings: new ListingStats(
				Total: listingsTotal,
				Active: listingsActive),
			ListingRequests: new ListingRequestStats(
				Total: listingRequestsTotal,
				Pending: listingRequestsPending),
			LeasesCount: leasesCount,
			WishlistsCount: wishlistsCount,
			OccupancyRate: occupancyRate);

		return response;
	}
}
