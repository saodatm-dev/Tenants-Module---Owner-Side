using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Meters.Update;

internal sealed class UpdateMeterCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IExecutionContextProvider executionContextProvider,
	IBuildingDbContext dbContext) : ICommandHandler<UpdateMeterCommand, Guid>
{
	public async Task<Result<Guid>> Handle(UpdateMeterCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.Meters.FirstOrDefaultAsync(item => item.Id == command.Id, cancellationToken);
		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.NotFound(nameof(UpdateMeterCommand.Id)));

		// check real estate ownership
		var hasAccess = await dbContext.RealEstates
			.AsNoTracking()
			.AnyAsync(item =>
				item.Id == maybeItem.RealEstateId &&
				item.OwnerId == executionContextProvider.TenantId, cancellationToken);

		// check lease if not direct owner
		if (!hasAccess)
		{
			hasAccess = await dbContext.Leases
				.AsNoTracking()
				.AnyAsync(item =>
					item.Items.Any(li => li.RealEstateId == maybeItem.RealEstateId) &&
					(item.OwnerId == executionContextProvider.TenantId ||
					 item.AgentId == executionContextProvider.TenantId ||
					 item.ClientId == executionContextProvider.TenantId), cancellationToken);
		}

		if (!hasAccess)
			return Result.Failure<Guid>(sharedViewLocalizer.RealEstateNotFound(nameof(UpdateMeterCommand.Id)));

		// check meter type
		if (!await dbContext.MeterTypes
					.AsNoTracking()
					.AnyAsync(item => item.Id == command.MeterTypeId, cancellationToken))
			return Result.Failure<Guid>(sharedViewLocalizer.MeterTypeNotFound(nameof(UpdateMeterCommand.MeterTypeId)));

		// check that no other meter of this type already exists for this real estate
		if (await dbContext.Meters
			.AsNoTracking()
			.AnyAsync(item => item.MeterTypeId == command.MeterTypeId && item.RealEstateId == maybeItem.RealEstateId && item.Id != command.Id, cancellationToken))
			return Result.Failure<Guid>(sharedViewLocalizer.AlreadyExists(nameof(UpdateMeterCommand.MeterTypeId)));

		dbContext.Meters.Update(
			maybeItem.Update(
				command.MeterTypeId,
				command.SerialNumber,
				command.InstallationDate,
				command.VerificationDate,
				command.NextVerificationDate,
				command.InitialReading ?? 0));

		await dbContext.SaveChangesAsync(cancellationToken);

		return maybeItem.Id;
	}
}
