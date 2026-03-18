using Building.Application.Core.Abstractions.Data;
using Building.Application.Core.Abstractions.Services;
using Building.Domain.Listings;
using Core.Application.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Abstractions.Services.Minio;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Listings.GetByPolygon;

internal sealed class GetListingsByPolygonQueryHandler(
	IBuildingDbContext dbContext,
	IGeometryService geometryService,
	IFileUrlResolver fileUrlResolver) : IQueryHandler<GetListingsByPolygonQuery, IEnumerable<GetListingsByPolygonResponse>>
{
	public async Task<Result<IEnumerable<GetListingsByPolygonResponse>>> Handle(GetListingsByPolygonQuery request, CancellationToken cancellationToken)
	{
		var polygon = geometryService.CreatePolygon(
					request.TopLeftLatitude,
					request.TopLeftLongitude,
					request.BottomRightLatitude,
					request.BottomRightLongitude);

		var listings = await dbContext.Listings
			.AsNoTracking()
			.IsActive()
			.IgnoreQueryFilters([IApplicationDbContext.TenantIdFilter])
			.Where(item => polygon != null ? polygon.Within(item.Location) : false)
			.Select(item => new GetListingsByPolygonResponse(
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
				item.Status))
			.ToListAsync(cancellationToken);

		var keys = listings.Select(item => item.Image).ToList();
		var resolvedUrls = await fileUrlResolver.ResolveManyAsync(keys, cancellationToken);
		var resolved = listings.Select((item, i) =>
			item with { Image = resolvedUrls[i] }).ToList();

		return resolved;
	}
}

