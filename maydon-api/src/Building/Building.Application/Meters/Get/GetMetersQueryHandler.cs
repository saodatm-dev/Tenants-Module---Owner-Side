using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Meters.Get;

internal sealed class GetMetersQueryHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IExecutionContextProvider executionContextProvider,
	IBuildingDbContext dbContext) : IQueryHandler<GetMetersQuery, IEnumerable<GetMetersResponse>>
{
	public async Task<Result<IEnumerable<GetMetersResponse>>> Handle(GetMetersQuery request, CancellationToken cancellationToken)
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
			return Result.Failure<IEnumerable<GetMetersResponse>>(sharedViewLocalizer.RealEstateNotFound(nameof(GetMetersQuery.RealEstateId)));

		return await dbContext.Meters
			.AsNoTrackingWithIdentityResolution()
			.Where(item => item.RealEstateId == request.RealEstateId)
			.Include(item => item.MeterType)
			.Select(item =>
				new GetMetersResponse(
					item.Id,
					item.MeterTypeId,
					dbContext.GetMeterTypeNameById(item.MeterTypeId),
					item.SerialNumber,
					item.InstallationDate,
					item.VerificationDate,
					item.NextVerificationDate,
					item.InitialReading))
			.ToListAsync(cancellationToken);
	}
}
