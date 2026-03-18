using Building.Application.Core.Abstractions.Data;
using Building.Domain.Listings;
using Core.Application.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Abstractions.Services.Minio;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Listings.GetMainPage;

internal sealed class GetMainPageListingsQueryHandler(
	IBuildingDbContext dbContext,
	IFileUrlResolver fileUrlResolver) : IQueryHandler<GetMainPageListingsQuery, IEnumerable<GetMainPageListingsResponse>>
{
	public async Task<Result<IEnumerable<GetMainPageListingsResponse>>> Handle(GetMainPageListingsQuery request, CancellationToken cancellationToken)
	{
		var mainPageListingCategories = await dbContext.ListingCategoryTranslates
			.AsNoTrackingWithIdentityResolution()
			.Include(item => item.ListingCategory)
			.Where(item => item.ListingCategory.ShowInMain)
			.OrderBy(item => item.ListingCategory.Order)
			.Select(item => new { item.ListingCategoryId, item.Value })
			.ToListAsync(cancellationToken);

		if (!mainPageListingCategories.Any())
			mainPageListingCategories = await dbContext.ListingCategoryTranslates
				.AsNoTrackingWithIdentityResolution()
				.Include(item => item.ListingCategory)
				.OrderBy(item => item.ListingCategory.Order)
				.Take(5)
				.Select(item => new { item.ListingCategoryId, item.Value })
				.ToListAsync(cancellationToken);

		var mainPageListingCategoryIds = mainPageListingCategories.Select(item => item.ListingCategoryId).ToArray();

		var query = await dbContext.Listings
			.AsNoTrackingWithIdentityResolution()
			.IsActive()
			.IgnoreQueryFilters([IApplicationDbContext.TenantIdFilter])
			.Include(item => item.RealEstate)
			.Select(item => new GetMainPageCategoryListingsResponse(
				item.Id,
				item.OwnerId,
				item.Title,
				item.CategoryIds.ToList(),
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
				item.Address))
			.ToListAsync(cancellationToken);

		//var uniqueKeys = query.Select(item => item.Image).Where(k => !string.IsNullOrWhiteSpace(k)).Distinct().ToList();
		//var resolvedUrls = await fileUrlResolver.ResolveManyAsync(uniqueKeys, cancellationToken);
		//var urlMap = uniqueKeys.Zip(resolvedUrls).ToDictionary(pair => pair.First, pair => pair.Second);

		var response = new List<GetMainPageListingsResponse>();

		foreach (var listingCategory in mainPageListingCategories)
		{
			var listings = query.Where(item => item.CategoryIds.Contains(listingCategory.ListingCategoryId)).ToList();
			if (listings.Any())
			{
				//var resolved = listings.Select(item =>
				//	item with { Image = urlMap.GetValueOrDefault(item.Image) ?? string.Empty }).ToList();

				response.Add(
					new GetMainPageListingsResponse(
						listingCategory.ListingCategoryId,
						listingCategory.Value,
						listings));
			}
		}

		return response;

		//return Result.Success(mainPageListingCategories
		//	.Select(category =>
		//		new GetMainPageListingsResponse(
		//			category.ListingCategoryId,
		//			category.Value,
		//			query.Where(item => item.CategoryIds.Contains(category.ListingCategoryId)))));
	}
}

