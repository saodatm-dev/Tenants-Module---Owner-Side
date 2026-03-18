using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Pagination;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.RealEstates.GetMy;

internal sealed class GetMyRealEstatesQueryHandler(
	IExecutionContextProvider executionContextProvider,
	IBuildingDbContext dbContext) : IQueryHandler<GetMyRealEstatesQuery, PagedList<GetMyRealEstatesResponse>>
{
	public async Task<Result<PagedList<GetMyRealEstatesResponse>>> Handle(GetMyRealEstatesQuery request, CancellationToken cancellationToken)
	{
		var query = dbContext.RealEstateDelegations
			.AsNoTracking()
			.Where(item => item.OwnerId == executionContextProvider.TenantId || item.AgentId == executionContextProvider.TenantId)
			.Include(item => item.RealEstate)
			.Where(item => item.RealEstate != null &&
			((request.RegionId != null && request.RegionId != Guid.Empty ? item.RealEstate.RegionId == request.RegionId : true) &&
			(request.DistrictId != null && request.DistrictId != Guid.Empty ? item.RealEstate.DistrictId == request.DistrictId : true) &&
			(!string.IsNullOrWhiteSpace(request.Filter)
				? EF.Functions.Like(item.RealEstate.BuildingNumber, $"%{request.Filter.ToLowerInvariant()}%") ||
				  EF.Functions.Like(item.RealEstate.Number, $"%{request.Filter.ToLowerInvariant()}%")
				: true)))
			.Select(item => new GetMyRealEstatesResponse(
				item.RealEstateId,
				item.OwnerId,
				item.RealEstate.ModerationStatus,
				item.RealEstate.BuildingNumber,
				item.RealEstate.FloorNumber,
				item.RealEstate.Number,
				item.RealEstate.RoomsCount,
				item.RealEstate.TotalArea,
				item.RealEstate.LivingArea,
				item.RealEstate.CeilingHeight,
				item.RealEstate.RegionId != null ? dbContext.GetRegionName(item.RealEstate.RegionId.Value) : string.Empty,
				item.RealEstate.DistrictId != null ? dbContext.GetDistrictName(item.RealEstate.DistrictId.Value) : string.Empty,
				item.RealEstate.Location != null ? item.RealEstate.Location.X : null,
				item.RealEstate.Location != null ? item.RealEstate.Location.Y : null,
				item.RealEstate.Address,
				item.RealEstate.Status,
				item.RealEstate.Reason));

		int totalCount = await query.CountAsync(cancellationToken);

		var responsesPage = await query
			.Skip(request.Page)
			.Take(request.PageSize)
			.ToListAsync(cancellationToken);

		return new PagedList<GetMyRealEstatesResponse>(responsesPage, request.Page, request.PageSize, totalCount);
	}
}
