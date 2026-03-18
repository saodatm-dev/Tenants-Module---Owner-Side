using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.MeterReadings.GetById;


internal sealed class GetMeterReadingByIdQueryHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IExecutionContextProvider executionContextProvider,
	IBuildingDbContext dbContext) : IQueryHandler<GetMeterReadingByIdQuery, GetMeterReadingByIdResponse>
{
	public async Task<Result<GetMeterReadingByIdResponse>> Handle(GetMeterReadingByIdQuery request, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.MeterReadings
			.AsNoTracking()
			.Where(item => item.Id == request.Id)
			.Select(item =>
				new GetMeterReadingByIdResponse(
					item.RealEstateId,
					item.MeterId,
					item.Meter != null ? dbContext.GetMeterTypeNameById(item.Meter.MeterTypeId) : string.Empty,
					item.Meter != null ? item.Meter.SerialNumber : string.Empty,
					item.ReadingDate,
					item.Value,
					item.PreviousValue,
					item.Consumption,
					item.IsManual))
			.FirstOrDefaultAsync(cancellationToken);

		if (maybeItem is null)
			return Result.Failure<GetMeterReadingByIdResponse>(sharedViewLocalizer.NotFound(nameof(GetMeterReadingByIdQuery.Id)));

		// check real estate
		var maybeRealEstate = await dbContext.RealEstates
			.AsNoTracking()
			.FirstOrDefaultAsync(item =>
				item.Id == maybeItem.RealEstateId &&
				item.OwnerId == executionContextProvider.TenantId, cancellationToken);

		// check lease
		if (maybeRealEstate is null)
		{
			var maybeLease = await dbContext.Leases
				.AsNoTracking()
				.FirstOrDefaultAsync(item =>
					item.Items.Any(li => li.RealEstateId == maybeItem.RealEstateId) &&
					(item.OwnerId == executionContextProvider.TenantId ||
					 item.AgentId == executionContextProvider.TenantId ||
					 item.ClientId == executionContextProvider.TenantId), cancellationToken);

			if (maybeLease is null)
				return Result.Failure<GetMeterReadingByIdResponse>(sharedViewLocalizer.NotFound(nameof(GetMeterReadingByIdQuery.Id)));
		}

		if (maybeRealEstate is null)
			return Result.Failure<GetMeterReadingByIdResponse>(sharedViewLocalizer.NotFound(nameof(GetMeterReadingByIdQuery.Id)));

		return maybeItem;
	}
}
