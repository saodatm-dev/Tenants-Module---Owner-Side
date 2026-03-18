using Building.Application.Core.Abstractions.Data;
using Building.Application.Listings.Get;
using Core.Application.Abstractions.Messaging;
using Core.Application.Pagination;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Wishlists.Get;

internal sealed class GetWishlistsQueryHandler(IBuildingDbContext dbContext) : IQueryHandler<GetWishlistsQuery, PagedList<GetListingsResponse>>
{
	public async Task<Result<PagedList<GetListingsResponse>>> Handle(GetWishlistsQuery request, CancellationToken cancellationToken)
	{
		var query = dbContext.Wishlists
			.AsNoTrackingWithIdentityResolution()
			.Join(dbContext.Listings,
				wishlist => wishlist.ListringId,
				listing => listing.Id,
				(wishlist, listing) => listing)
			.Where(item => !string.IsNullOrWhiteSpace(request.Filter)
				? ((!string.IsNullOrEmpty(item.ComplexName) ? EF.Functions.Like(item.ComplexName.ToLower(), $"%{request.Filter.ToLowerInvariant()}%") : true) ||
				  (!string.IsNullOrEmpty(item.BuildingNumber) ? EF.Functions.Like(item.BuildingNumber.ToLower(), $"%{request.Filter.ToLowerInvariant()}%") : true) ||
				  (!string.IsNullOrEmpty(item.Address) ? EF.Functions.Like(item.Address.ToLower(), $"%{request.Filter.ToLowerInvariant()}%") : true))
				: true)
			.Select(item => new GetListingsResponse(
				item.Id,
				item.OwnerId,
				item.Title,
				item.CategoryIds.ToList(),
				dbContext.GetListingCategoryNamesByIds(item.CategoryIds),
				item.RealEstate.Images != null && item.RealEstate.Images.Any() ? item.RealEstate.Images.First().ObjectName : string.Empty,
				item.ComplexName,
				item.BuildingNumber,
				item.TotalArea,
				item.FloorIds != null && item.FloorIds.Any() ? item.FloorIds.Count() : null,
				item.Description,
				item.PriceForMonth,
				item.PricePerSquareMeter,
				item.RegionId != null ? dbContext.GetRegionName(item.RegionId.Value) : string.Empty,
				item.DistrictId != null ? dbContext.GetDistrictName(item.DistrictId.Value) : string.Empty,
				item.Location != null ? item.Location.X : null,
				item.Location != null ? item.Location.Y : null,
				item.Address,
				item.Status));

		int totalCount = await query.CountAsync(cancellationToken);

		var responsesPage = await query
			.Skip(request.Page)
			.Take(request.PageSize)
			.ToListAsync(cancellationToken);

		return new PagedList<GetListingsResponse>(responsesPage, request.Page, request.PageSize, totalCount);
	}
}
