using Building.Application.Core.Abstractions.Data;
using Building.Domain.Listings;
using Core.Application.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Abstractions.Services.Minio;
using Core.Application.Pagination;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Listings.Get;

internal sealed class GetListingsQueryHandler(
	IBuildingDbContext dbContext,
	IFileUrlResolver fileUrlResolver) : IQueryHandler<GetListingsQuery, PagedList<GetListingsResponse>>
{
	public async Task<Result<PagedList<GetListingsResponse>>> Handle(GetListingsQuery request, CancellationToken cancellationToken)
	{
		var query = dbContext.Listings
			.AsNoTracking()
			.IsActive()
			.IgnoreQueryFilters([IApplicationDbContext.TenantIdFilter])
			.Where(item =>
			((request.CategoryId != null && request.CategoryId != Guid.Empty ? EF.Functions.JsonExistAny(item.CategoryIds, $"{request.CategoryId}") : true) &&
			(request.RegionId != null && request.RegionId != Guid.Empty ? item.RegionId == request.RegionId : true) &&
			(request.DistrictId != null && request.DistrictId != Guid.Empty ? item.DistrictId == request.DistrictId : true) &&
			(request.RoomsCount != null && request.RoomsCount > 0 ? item.RoomsCount == request.RoomsCount : true) &&
			(request.FromFloorNumber != null && request.ToFloorNumber != null && item.FloorNumbers != null && item.FloorNumbers.Any()
				? item.FloorNumbers.Any(floorNumber => request.FromFloorNumber <= floorNumber && request.ToFloorNumber >= floorNumber)
				: true) &&
			(request.FromSquare != null && request.ToSquare != null
				? request.FromSquare <= item.TotalArea && request.ToFloorNumber >= item.TotalArea
				: true) &&
			(request.FromPrice != null && request.ToPrice != null
				? (request.FromPrice <= item.PricePerSquareMeter || request.FromPrice <= item.PriceForMonth) &&
				  (request.ToPrice >= item.PriceForMonth)
				: true) &&
			(!string.IsNullOrWhiteSpace(request.Filter)
				? (!string.IsNullOrEmpty(item.BuildingNumber) ? EF.Functions.Like(item.BuildingNumber.ToLower(), $"%{request.Filter.ToLowerInvariant()}%") : false) ||
				  (!string.IsNullOrEmpty(item.Address) ? EF.Functions.Like(item.Address.ToLower(), $"%{request.Filter.ToLowerInvariant()}%") : false) ||
				  (!string.IsNullOrEmpty(item.ComplexName) ? EF.Functions.Like(item.ComplexName.ToLower(), $"%{request.Filter.ToLowerInvariant()}%") : false)
				: true)))
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

		var responsesFiltered = query
			.Where(item => request.CategoryId != null && request.CategoryId != Guid.Empty ? item.CategoryIds.Contains(request.CategoryId.Value) : true)
			.ToList();

		int totalCount = responsesFiltered.Count;

		var responsesPage = responsesFiltered
			.Skip(request.Page)
			.Take(request.PageSize)
			.ToList();

		var keys = responsesPage.Select(item => item.Image).ToList();
		var resolvedUrls = await fileUrlResolver.ResolveManyAsync(keys, cancellationToken);
		var resolvedPage = responsesPage.Select((item, i) =>
			item with { Image = resolvedUrls[i] }).ToList();

		return new PagedList<GetListingsResponse>(resolvedPage, request.Page, request.PageSize, totalCount);

	}
}

