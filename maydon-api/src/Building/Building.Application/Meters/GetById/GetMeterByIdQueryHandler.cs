using Building.Application.Core.Abstractions.Data;
using Building.Application.Meters.Get;
using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Meters.GetById;

internal sealed class GetMeterByIdQueryHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IExecutionContextProvider executionContextProvider,
	IBuildingDbContext dbContext) : IQueryHandler<GetMeterByIdQuery, GetMeterByIdResponse>
{
	public async Task<Result<GetMeterByIdResponse>> Handle(GetMeterByIdQuery request, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.Meters
			.AsNoTracking()
			.Where(item => item.Id == request.Id)
			.Select(item =>
				new GetMeterByIdResponse(
					item.RealEstateId,
					item.MeterTypeId,
					dbContext.GetMeterTypeNameById(item.MeterTypeId),
					item.SerialNumber,
					item.InstallationDate,
					item.VerificationDate,
					item.NextVerificationDate,
					item.InitialReading))
			.FirstOrDefaultAsync(cancellationToken);

		if (maybeItem is null)
			return Result.Failure<GetMeterByIdResponse>(sharedViewLocalizer.NotFound(nameof(GetMeterByIdQuery.Id)));

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
				return Result.Failure<GetMeterByIdResponse>(sharedViewLocalizer.RealEstateNotFound(nameof(GetMetersQuery.RealEstateId)));
		}

		if (maybeRealEstate is null)
			return Result.Failure<GetMeterByIdResponse>(sharedViewLocalizer.RealEstateNotFound(nameof(GetMetersQuery.RealEstateId)));

		return maybeItem;
	}
}
