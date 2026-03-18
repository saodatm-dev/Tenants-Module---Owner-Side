using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Abstractions.Services.Minio;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Listings.GetById;

internal sealed class GetListingByIdQueryHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IBuildingDbContext dbContext,
	IFileUrlResolver fileUrlResolver) : IQueryHandler<GetListingByIdQuery, GetListingByIdResponse>
{
	public async Task<Result<GetListingByIdResponse>> Handle(GetListingByIdQuery request, CancellationToken cancellationToken)
	{
		var listing = await dbContext.Listings
			.AsNoTrackingWithIdentityResolution()
			.IgnoreQueryFilters([IApplicationDbContext.TenantIdFilter])
			.Where(item => item.Id == request.Id)
			.Include(item => item.RealEstate)
				.ThenInclude(re => re.Images)
			.Include(item => item.ListingAmenities)
			.FirstOrDefaultAsync(cancellationToken);

		if (listing is null)
			return Result.Failure<GetListingByIdResponse>(sharedViewLocalizer.NotFound(nameof(GetListingByIdQuery.Id)));

		IEnumerable<GetListingByIdFloorResponse>? floorResponses = null;
		if (listing.FloorIds?.Count() > 0)
		{
			var floors = await dbContext.Floors
				.AsNoTracking()
				.Where(f => listing.FloorIds.Contains(f.Id))
				.Select(floor => new GetListingByIdFloorResponse(
					floor.Id,
					floor.Number,
					floor.Type,
					floor.Label,
					floor.TotalArea,
					floor.CeilingHeight,
					floor.Plan))
				.ToListAsync(cancellationToken);

			floorResponses = floors.Count > 0 ? floors : null;
		}

		var listingAmenityIds = listing.ListingAmenities?.Select(item => item.AmenityId);

		var rentalPurposeName = listing.RentalPurposeId.HasValue
			? await dbContext.RentalPurposeTranslates
				.AsNoTracking()
				.Where(t => t.RentalPurposeId == listing.RentalPurposeId.Value)
				.Select(t => t.Value)
				.FirstOrDefaultAsync(cancellationToken)
			: null;

		var translatedDescription = await dbContext.ListingTranslates
			.AsNoTracking()
			.Where(t => t.ListingId == listing.Id)
			.Select(t => t.Value)
			.FirstOrDefaultAsync(cancellationToken);

		var imageKeys = listing.RealEstate.Images?.Select(img => img.ObjectName);
		//var images = (await fileUrlResolver.ResolveUrlsAsync(imageKeys, cancellationToken)).ToList();

		return new GetListingByIdResponse(
			listing.Id,
			listing.OwnerId,
			listing.Title,
			dbContext.GetListingCategoryNamesByIds(listing.CategoryIds),
			listing.ComplexName,
			listing.BuildingNumber,
			floorResponses,
			listing.RoomsCount,
			translatedDescription ?? listing.Description,
			listingAmenityIds?.Any() == true ? dbContext.GetListingAmenitiesByIds(listingAmenityIds) : null,
			listing.RealEstate.Plan,
			listing.TotalArea,
			listing.LivingArea,
			listing.CeilingHeight,
			listing.PriceForMonth,
			listing.PricePerSquareMeter,
			listing.RegionId != null ? dbContext.GetRegionName(listing.RegionId.Value) : string.Empty,
			listing.DistrictId != null ? dbContext.GetDistrictName(listing.DistrictId.Value) : string.Empty,
			listing.Location != null ? listing.Location.X : null,
			listing.Location != null ? listing.Location.Y : null,
			listing.Address,
			listing.Status,
			imageKeys,
			rentalPurposeName,
			listing.MinLeaseTerm,
			listing.UtilityPaymentType,
			listing.NextAvailableDate);
	}
}
