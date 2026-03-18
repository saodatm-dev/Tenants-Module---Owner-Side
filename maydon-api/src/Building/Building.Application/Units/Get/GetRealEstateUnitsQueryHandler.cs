using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Pagination;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Units.Get;


internal sealed class GetRealEstateUnitsQueryHandler(IBuildingDbContext dbContext)
	: IQueryHandler<GetRealEstateUnitsQuery, PagedList<GetRealEstateUnitsResponse>>
{
	public async Task<Result<PagedList<GetRealEstateUnitsResponse>>> Handle(
		GetRealEstateUnitsQuery request,
		CancellationToken cancellationToken)
	{
		var query = dbContext.Units
			.Where(item =>
				(request.RealEstateId != null ? item.RealEstateId == request.RealEstateId : true) &&
				(request.RealEstateTypeId != null ? item.RealEstateTypeId == request.RealEstateTypeId : true) &&
				(request.RenovationId != null ? item.RenovationId == request.RenovationId : true) &&
				(request.FloorNumber != null ? item.FloorNumber == request.FloorNumber : true))
			//(!string.IsNullOrEmpty(request.Filter) ? (!string.IsNullOrEmpty(item.Number) && EF.Functions.ILike(item.Number, $"%{request.Filter.ToLowerInvariant()}%")) : true))
			.OrderBy(item => item.FloorNumber ?? 0)//.ThenBy(x => x.Number)
			.Select(item => new GetRealEstateUnitsResponse(
				item.Id,
				item.RealEstateId,
				item.RealEstateTypeId,
				item.FloorNumber,
				item.RoomNumber,
				item.TotalArea,
				item.CeilingHeight,
				item.RenovationId,
				null,
				item.Plan,
				item.Coordinates,
				item.Reason,
				item.Status,
				item.ModerationStatus));

		int totalCount = await query.CountAsync(cancellationToken);

		var responsesPage = await query
			.Skip(request.Page)
			.Take(request.PageSize)
			.ToListAsync(cancellationToken);

		return new PagedList<GetRealEstateUnitsResponse>(responsesPage, request.Page, request.PageSize, totalCount);
	}
}
