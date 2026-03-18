using Building.Application.Core.Abstractions.Data;
using Building.Domain.Meters;
using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Meters.Create;

internal sealed class CreateMeterCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IExecutionContextProvider executionContextProvider,
	IBuildingDbContext dbContext) : ICommandHandler<CreateMeterCommand, Guid>
{
	public async Task<Result<Guid>> Handle(CreateMeterCommand command, CancellationToken cancellationToken)
	{
		// check real estate ownership or lease access
		var hasAccess = await dbContext.RealEstates
			.AsNoTracking()
			.AnyAsync(item =>
				item.Id == command.RealEstateId &&
				item.OwnerId == executionContextProvider.TenantId, cancellationToken);

		// check lease if not direct owner
		if (!hasAccess)
		{
			hasAccess = await dbContext.Leases
				.AsNoTracking()
				.AnyAsync(item =>
					item.Items.Any(li => li.RealEstateId == command.RealEstateId) &&
					(item.OwnerId == executionContextProvider.TenantId ||
					 item.AgentId == executionContextProvider.TenantId ||
					 item.ClientId == executionContextProvider.TenantId), cancellationToken);
		}

		if (!hasAccess)
			return Result.Failure<Guid>(sharedViewLocalizer.RealEstateNotFound(nameof(CreateMeterCommand.RealEstateId)));

		// check if meter of this type already exists for this real estate
		if (await dbContext.Meters
			.AsNoTracking()
			.AnyAsync(item => item.MeterTypeId == command.MeterTypeId && item.RealEstateId == command.RealEstateId, cancellationToken))
			return Result.Failure<Guid>(sharedViewLocalizer.AlreadyExists(nameof(CreateMeterCommand.MeterTypeId)));

		// check meter type
		if (!await dbContext.MeterTypes
					.AsNoTracking()
					.AnyAsync(item => item.Id == command.MeterTypeId, cancellationToken))
			return Result.Failure<Guid>(sharedViewLocalizer.MeterTypeNotFound(nameof(CreateMeterCommand.MeterTypeId)));

		var item = new Meter(
			command.RealEstateId,
			null,
			command.MeterTypeId,
			command.SerialNumber,
			command.InstallationDate,
			command.VerificationDate,
			command.NextVerificationDate,
			command.InitialReading ?? 0);

		await dbContext.Meters.AddAsync(item, cancellationToken);

		await dbContext.SaveChangesAsync(cancellationToken);

		return item.Id;
	}
}
