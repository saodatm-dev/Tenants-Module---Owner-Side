using Building.Application.Core.Abstractions.Data;
using Building.Domain.Buildings;
using Core.Application.Abstractions.Messaging;
using Core.Application.Abstractions.Services.Minio;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Buildings.GetById;

internal sealed class GetBuildingByIdQueryHandler(
	IBuildingDbContext dbContext,
	IFileUrlResolver fileUrlResolver) : IQueryHandler<GetBuildingByIdQuery, GetBuildingByIdResponse>
{
	public async Task<Result<GetBuildingByIdResponse>> Handle(GetBuildingByIdQuery request, CancellationToken cancellationToken)
	{
		var building = await dbContext.Buildings
			.AsNoTrackingWithIdentityResolution()
			.Where(item => item.Id == request.Id)
			.IsActive()
			.Include(item => item.Floors)
			.ThenInclude(item => item.RealEstates)
			.Include(item => item.Descriptions)
			.Include(item => item.Images)
			.Select(item => new GetBuildingByIdResponse(
				item.Id,
				item.ComplexId,
				item.RegionId,
				item.DistrictId,
				item.Number,
				item.Descriptions != null && item.Descriptions.Any() ? item.Descriptions.First().Value : string.Empty,
				item.IsCommercial,
				item.IsLiving,
				item.Location != null ? item.Location.X : null,
				item.Location != null ? item.Location.Y : null,
				item.Address,
				item.TotalArea,
				item.Images != null && item.Images.Any() ? item.Images.Select(img => img.ObjectName) : null,
				item.Floors.Select(floor =>
					new Floors.Get.GetFloorsResponse(
						floor.Id,
						item.Number,
						dbContext.GetBulkCategoryBuildingTypes(floor.RealEstates.Select(item => item.RealEstateTypeId)),
						floor.Number,
						floor.Type,
						floor.Label,
						floor.TotalArea,
						floor.CeilingHeight,
                        floor.Plan))))
			.AsSplitQuery()
			.FirstOrDefaultAsync(cancellationToken);

		if (building is null)
			return Result<GetBuildingByIdResponse>.None;

		var resolvedImages = await fileUrlResolver.ResolveUrlsAsync(building.Images, cancellationToken);
		return building with { Images = resolvedImages };
	}
}

