using Building.Application.Core.Abstractions.Data;
using Building.Domain.Buildings;
using Core.Application.Abstractions.Messaging;
using Core.Application.Abstractions.Services.Minio;
using Core.Application.Pagination;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Buildings.Get;

internal sealed class GetBuildingsQueryHandler(
	IBuildingDbContext dbContext,
	IFileUrlResolver fileUrlResolver) : IQueryHandler<GetBuildingsQuery, PagedList<GetBuildingsResponse>>
{
	public async Task<Result<PagedList<GetBuildingsResponse>>> Handle(GetBuildingsQuery request, CancellationToken cancellationToken)
	{
		var query = dbContext.Buildings
			.AsNoTrackingWithIdentityResolution()
			.Where(item =>
				!string.IsNullOrWhiteSpace(request.Filter)
				? EF.Functions.Like(item.Number.ToLower(), $"%{request.Filter.ToLowerInvariant()}%")
				: true)
			.IsActive()
			.Include(item => item.Complex)
			.Include(item => item.Floors)
			.ThenInclude(item => item.RealEstates)
			.Include(item => item.Descriptions)
			.Include(item => item.Images)
			.Select(item => new GetBuildingsResponse(
				item.Id,
				item.Complex != null ? item.Complex.Name : null,
				item.RegionId != null ? dbContext.GetRegionName(item.RegionId.Value) : string.Empty,
				item.DistrictId != null ? dbContext.GetDistrictName(item.DistrictId.Value) : string.Empty,
				item.Number,
				item.Descriptions != null && item.Descriptions.Any() ? item.Descriptions.First().Value : string.Empty,
				item.IsCommercial,
				item.IsLiving,
				item.Location != null ? item.Location.X : null,
				item.Location != null ? item.Location.Y : null,
				item.Address,
				item.TotalArea,
				item.Images != null && item.Images.Any(image => !string.IsNullOrEmpty(image.ObjectName))
					? item.Images.Where(image => !string.IsNullOrEmpty(image.ObjectName)).Select(img => img.ObjectName)
					: null,
				item.Floors.Count > 0
				? item.Floors.Select(floor =>
					new Floors.Get.GetFloorsResponse(
						floor.Id,
						item.Number,
						dbContext.GetBulkCategoryBuildingTypes(floor.RealEstates.Select(item => item.RealEstateTypeId)),
						floor.Number,
						floor.Type,
						floor.Label,
						floor.TotalArea,
						floor.CeilingHeight,
                        floor.Plan))
				: null))
			.AsSplitQuery();

		int totalCount = await query.CountAsync(cancellationToken);

		var responsesPage = await query
			.Skip(request.Page)
			.Take(request.PageSize)
			.ToListAsync(cancellationToken);

		var tasks = responsesPage.Select(async item =>
		{
			var resolvedImages = await fileUrlResolver.ResolveUrlsAsync(item.Images, cancellationToken);
			return item with { Images = resolvedImages };
		});
		var resolvedPage = (await Task.WhenAll(tasks)).ToList();

		return new PagedList<GetBuildingsResponse>(resolvedPage, request.Page, request.PageSize, totalCount);
	}
}
