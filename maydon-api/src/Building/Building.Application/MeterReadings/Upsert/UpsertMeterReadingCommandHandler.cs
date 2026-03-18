using Building.Application.Core.Abstractions.Data;
using Building.Domain.MeterReadings;
using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Providers;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.MeterReadings.Upsert;

internal sealed class UpsertMeterReadingCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IDateTimeProvider dateTimeProvider,
	IExecutionContextProvider executionContextProvider,
	IBuildingDbContext dbContext) : ICommandHandler<UpsertMeterReadingCommand, Guid>
{
	public async Task<Result<Guid>> Handle(UpsertMeterReadingCommand command, CancellationToken cancellationToken)
	{
		var maybeMeter = await dbContext.Meters.FirstOrDefaultAsync(item => item.Id == command.MeterId, cancellationToken);
		if (maybeMeter is null)
			return Result.Failure<Guid>(sharedViewLocalizer.NotFound(nameof(UpsertMeterReadingCommand.MeterId)));

		// check real estate ownership or lease access
		var hasAccess = await dbContext.RealEstates
			.AsNoTracking()
			.AnyAsync(item =>
				item.Id == maybeMeter.RealEstateId &&
				item.OwnerId == executionContextProvider.TenantId, cancellationToken);

		// check lease if not direct owner
		if (!hasAccess)
		{
			hasAccess = await dbContext.Leases
				.AsNoTracking()
				.AnyAsync(item =>
					item.Items.Any(li => li.RealEstateId == maybeMeter.RealEstateId) &&
					(item.OwnerId == executionContextProvider.TenantId ||
					 item.AgentId == executionContextProvider.TenantId), cancellationToken);
		}

		if (!hasAccess)
			return Result.Failure<Guid>(sharedViewLocalizer.RealEstateNotFound(nameof(UpsertMeterReadingCommand.MeterId)));

		var maybeItem = await dbContext.MeterReadings.FirstOrDefaultAsync(item => item.MeterId == command.MeterId, cancellationToken);

		var currentValue = command.Value;
		var previousValue = maybeItem?.PreviousValue > 0 ? maybeItem.PreviousValue : command.PreviousValue ?? 0;

		if (maybeItem is null)
		{
			maybeItem = new MeterReading(
				command.MeterId,
				maybeMeter.RealEstateId,
				command.ReadingDate ?? DateOnly.FromDateTime(dateTimeProvider.UtcNow),
				currentValue,
				previousValue,
				currentValue - previousValue,
				command.IsManual,
				command.Note);

			await dbContext.MeterReadings.AddAsync(maybeItem, cancellationToken);
		}
		else
		{
			maybeItem.Update(
				command.ReadingDate ?? DateOnly.FromDateTime(dateTimeProvider.UtcNow),
				currentValue,
				previousValue,
				currentValue - previousValue,
				command.IsManual,
				command.Note);

			dbContext.MeterReadings.Update(maybeItem);
		}

		await dbContext.SaveChangesAsync(cancellationToken);

		return maybeMeter.Id;
	}
}
