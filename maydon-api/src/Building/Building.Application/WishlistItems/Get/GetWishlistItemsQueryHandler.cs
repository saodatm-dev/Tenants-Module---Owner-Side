using Building.Application.Core.Abstractions.Data;
using Building.Application.RealEstates.Get;
using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Pagination;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.WishlistItems.Get;

internal sealed class GetWishlistItemsQueryHandler(
	IExecutionContextProvider executionContextProvider,
	IBuildingDbContext dbContext) : IQueryHandler<GetWishlistItemsQuery, PagedList<GetWishlistItemsResponse>>
{
	public async Task<Result<PagedList<GetWishlistItemsResponse>>> Handle(GetWishlistItemsQuery request, CancellationToken cancellationToken)
	{
		var query = dbContext.WishlistItems
			.Include(item => item.Wishlist)
			.Where(item => item.Wishlist.TenantId == executionContextProvider.TenantId &&
								  item.Wishlist.UserId == executionContextProvider.UserId)
			.Include(item => item.RealEstate)
			.Where(item =>
					!string.IsNullOrWhiteSpace(request.Filter) && !string.IsNullOrWhiteSpace(item.RealEstate.Address)
					? EF.Functions.Like(item.RealEstate.Address.ToLower(), $"%{request.Filter.ToLowerInvariant()}%")
					: true)
			.Select(item => new GetWishlistItemsResponse(
				item.Id,
				new GetRealEstatesResponse(
					item.RealEstate.Id,
					item.RealEstate.TenantId,
					item.RealEstate.BuildingNumber,
					item.RealEstate.FloorNumber,
					item.RealEstate.Number,
					item.RealEstate.RoomsCount,
					item.RealEstate.Square,
					item.RealEstate.LivingSquare,
					item.RealEstate.CeilingHeight,
					item.RealEstate.RegionId != null ? dbContext.GetRegionName(item.RealEstate.RegionId.Value) : string.Empty,
					item.RealEstate.DistrictId != null ? dbContext.GetDistrictName(item.RealEstate.DistrictId.Value) : string.Empty,
					item.RealEstate.Latitude,
					item.RealEstate.Longitude,
					item.RealEstate.Address)))
			.AsNoTracking();

		int totalCount = await query.CountAsync(cancellationToken);

		var responsesPage = await query
			.Skip(request.Page)
			.Take(request.PageSize)
			.ToListAsync(cancellationToken);

		return new PagedList<GetWishlistItemsResponse>(responsesPage, request.Page, request.PageSize, totalCount);
	}
}
