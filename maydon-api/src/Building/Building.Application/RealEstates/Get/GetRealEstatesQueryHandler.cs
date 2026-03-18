using Building.Application.Core.Abstractions.Data;
using Building.Domain.RealEstates;
using Core.Application.Abstractions.Messaging;
using Core.Application.Abstractions.Services.Minio;
using Core.Application.Pagination;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.RealEstates.Get;

internal sealed class GetRealEstatesQueryHandler(
	IBuildingDbContext dbContext,
	IFileUrlResolver fileUrlResolver) : IQueryHandler<GetRealEstatesQuery, PagedList<GetRealEstatesResponse>>
{
	public async Task<Result<PagedList<GetRealEstatesResponse>>> Handle(GetRealEstatesQuery request, CancellationToken cancellationToken)
	{
		var query = (from realEstate in dbContext.RealEstates
					 .AsNoTracking()
					 .OrderBy(item => item.CreatedAt)
					 .IsActive()
					 .Include(item => item.Units)
					 .Include(item => item.Images)
					 .Where(item =>

					 (request.RealEstateTypeId != null && request.RealEstateTypeId != Guid.Empty ? item.RealEstateTypeId == request.RealEstateTypeId : true) &&
					 (request.BuildingId != null && request.BuildingId != Guid.Empty ? item.BuildingId == request.BuildingId : true) &&
					 (request.RegionId != null && request.RegionId != Guid.Empty ? item.RegionId == request.RegionId : true) &&
					 (request.DistrictId != null && request.DistrictId != Guid.Empty ? item.DistrictId == request.DistrictId : true) &&
					 (request.RoomsCount != null && request.RoomsCount > 0 ? item.RoomsCount == request.RoomsCount : true) &&
					 (request.FloorNumber != null ? item.FloorNumber == request.FloorNumber || item.Units.Any(u => u.FloorNumber == request.FloorNumber) : true) &&
					 (!string.IsNullOrWhiteSpace(request.Filter)
						? EF.Functions.Like(item.BuildingNumber, $"%{request.Filter.ToLowerInvariant()}%") &&
						  EF.Functions.Like(item.Number, $"%{request.Filter.ToLowerInvariant()}%")
						 : true))

					 let typeName = dbContext.RealEstateTypeTranslates
							  .Where(item =>
								  item.RealEstateTypeId == realEstate.RealEstateTypeId &&
								  item.Field == Domain.RealEstateTypes.RealEstateTypeField.Name)
							  .Select(item => item.Value)
							  .FirstOrDefault()

					 select new GetRealEstatesResponse(
						 realEstate.Id,
						 realEstate.OwnerId,
						 typeName,
						 realEstate.BuildingNumber,
						 realEstate.FloorNumber,
						 realEstate.Number,
						 realEstate.RoomsCount,
						 realEstate.TotalArea,
						 realEstate.LivingArea,
						 realEstate.CeilingHeight,
						 realEstate.AboveFloors,
						 realEstate.BelowFloors,
						 realEstate.RegionId != null ? dbContext.GetRegionName(realEstate.RegionId.Value) : string.Empty,
						 realEstate.DistrictId != null ? dbContext.GetDistrictName(realEstate.DistrictId.Value) : string.Empty,
						 realEstate.Location != null ? realEstate.Location.X : null,
						 realEstate.Location != null ? realEstate.Location.Y : null,
						 realEstate.Address,
						 realEstate.Plan,
						 realEstate.Images != null ? realEstate.Images.Select(image => image.ObjectName) : Enumerable.Empty<string>()));

		int totalCount = await query.CountAsync(cancellationToken);

		var responsesPage = await query
			.Skip(request.Page)
			.Take(request.PageSize)
			.ToListAsync(cancellationToken);

		var tasks = responsesPage.Select(async item =>
		{
			var resolvedImages = await fileUrlResolver.ResolveUrlsAsync(item.Images, cancellationToken);
			var resolvedPlan = await fileUrlResolver.ResolveUrlAsync(item.Plan, cancellationToken);
			return item with { Images = resolvedImages, Plan = resolvedPlan };
		});
		var resolvedPage = (await Task.WhenAll(tasks)).ToList();

		return new PagedList<GetRealEstatesResponse>(resolvedPage, request.Page, request.PageSize, totalCount);
	}
}
