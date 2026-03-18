using Building.Application.Core.Abstractions.Data;
using Building.Application.Meters.Get;
using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Pagination;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.MeterReadings.Get;

internal sealed class GetMeterReadingsQueryHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IExecutionContextProvider executionContextProvider,
	IBuildingDbContext dbContext) : IQueryHandler<GetMeterReadingsQuery, PagedList<GetMeterReadingsResponse>>
{
	public async Task<Result<PagedList<GetMeterReadingsResponse>>> Handle(GetMeterReadingsQuery request, CancellationToken cancellationToken)
	{
		// check real estate ownership
		var hasAccess = await dbContext.RealEstates
			.AsNoTracking()
			.AnyAsync(item =>
				item.Id == request.RealEstateId &&
				item.OwnerId == executionContextProvider.TenantId, cancellationToken);

		// check lease if not direct owner
		if (!hasAccess)
		{
			hasAccess = await dbContext.Leases
				.AsNoTracking()
				.AnyAsync(item =>
					item.Items.Any(li => li.RealEstateId == request.RealEstateId) &&
					(item.OwnerId == executionContextProvider.TenantId ||
					 item.AgentId == executionContextProvider.TenantId ||
					 item.ClientId == executionContextProvider.TenantId), cancellationToken);
		}

		if (!hasAccess)
			return Result.Failure<PagedList<GetMeterReadingsResponse>>(sharedViewLocalizer.RealEstateNotFound(nameof(GetMetersQuery.RealEstateId)));

		var query = dbContext.MeterReadings
			.AsNoTrackingWithIdentityResolution()
			.Where(item =>
				item.RealEstateId == request.RealEstateId &&
				(request.MeterId != null && request.MeterId != Guid.Empty ? item.MeterId == request.MeterId : true))
			.Include(item => item.Meter)
			.Where(item => request.MeterTypeId != null && request.MeterTypeId != Guid.Empty && item.Meter != null ? item.Meter.MeterTypeId == request.MeterTypeId : true)
			.Select(item =>
				new GetMeterReadingsResponse(
					item.MeterId,
					item.Meter != null ? dbContext.GetMeterTypeNameById(item.Meter.MeterTypeId) : string.Empty,
					item.Meter != null ? item.Meter.SerialNumber : string.Empty,
					item.ReadingDate,
					item.Value,
					item.PreviousValue,
					item.Consumption,
					item.IsManual));

		int totalCount = await query.CountAsync(cancellationToken);

		var responsesPage = await query
			.Skip(request.Page)
			.Take(request.PageSize)
			.ToListAsync(cancellationToken);

		return new PagedList<GetMeterReadingsResponse>(responsesPage, request.Page, request.PageSize, totalCount);
	}
}
