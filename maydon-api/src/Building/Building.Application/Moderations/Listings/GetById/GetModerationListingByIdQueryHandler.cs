using Building.Application.Core.Abstractions.Data;
using Building.Application.Listings.GetById;
using Core.Application.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Abstractions.Services.Minio;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Moderations.Listings.GetById;

internal sealed class GetModerationListingByIdQueryHandler(
	IBuildingDbContext dbContext,
	IFileUrlResolver fileUrlResolver) : IQueryHandler<GetModerationListingByIdQuery, GetModerationListingByIdResponse>
{
	public async Task<Result<GetModerationListingByIdResponse>> Handle(GetModerationListingByIdQuery request, CancellationToken cancellationToken)
	{
		var listing = await dbContext.Listings
			.AsNoTracking()
			.IgnoreQueryFilters([IApplicationDbContext.TenantIdFilter])
			.Include(item => item.RealEstate)
				.ThenInclude(re => re.Images)
			.Where(item => item.Id == request.Id)
			.FirstOrDefaultAsync(cancellationToken);

		if (listing is null)
			return Result<GetModerationListingByIdResponse>.None;

		// Fetch floors separately to avoid EF Core translation issues with FloorIds.Contains()
		IEnumerable<GetListingByIdFloorResponse>? floorResponses = null;
		if (listing.FloorIds != null && listing.FloorIds.Any())
		{
			var floorIdsList = listing.FloorIds.ToList();
			floorResponses = await dbContext.Floors
				.AsNoTracking()
				.Where(f => floorIdsList.Contains(f.Id))
				.Select(f => new GetListingByIdFloorResponse(
					f.Id, f.Number, f.Type, f.Label, f.TotalArea, f.CeilingHeight, f.Plan))
				.ToListAsync(cancellationToken);
		}

		var objectNames = listing.RealEstate?.Images != null && listing.RealEstate.Images.Any()
			? listing.RealEstate.Images.Select(image => image.ObjectName)
			: null;
		var resolvedImages = await fileUrlResolver.ResolveUrlsAsync(objectNames, cancellationToken);

		return new GetModerationListingByIdResponse(
			listing.Id,
			listing.OwnerId,
			listing.Status,
			listing.ModerationStatus,
			listing.Reason,
			dbContext.GetCategoryNamesByIds(listing.CategoryIds.ToList()),
			listing.ComplexName,
			listing.BuildingNumber,
			floorResponses,
			listing.FloorNumbers,
			listing.RoomsCount,
			listing.TotalArea,
			listing.LivingArea,
			listing.CeilingHeight,
			listing.PriceForMonth,
			listing.PricePerSquareMeter,
			listing.Description,
			listing.RegionId != null ? dbContext.GetRegionName(listing.RegionId.Value) : string.Empty,
			listing.DistrictId != null ? dbContext.GetDistrictName(listing.DistrictId.Value) : string.Empty,
			listing.Location != null ? listing.Location.X : null,
			listing.Location != null ? listing.Location.Y : null,
			listing.Address,
			listing.CreatedAt,
			resolvedImages);
	}
}
