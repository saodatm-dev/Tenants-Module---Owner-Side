using Building.Application.Core.Abstractions.Data;
using Building.Application.Meters.Create;
using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Meters.Remove;

internal sealed class RemoveMeterCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IExecutionContextProvider executionContextProvider,
	IBuildingDbContext dbContext) : ICommandHandler<RemoveMeterCommand>
{
	public async Task<Result> Handle(RemoveMeterCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.Meters.FirstOrDefaultAsync(item => item.Id == command.Id, cancellationToken);
		if (maybeItem is null)
			return Result.Failure(sharedViewLocalizer.NotFound(nameof(RemoveMeterCommand.Id)));

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
				return Result.Failure(sharedViewLocalizer.RealEstateNotFound(nameof(CreateMeterCommand.RealEstateId)));
		}

		if (maybeRealEstate is null)
			return Result.Failure(sharedViewLocalizer.RealEstateNotFound(nameof(CreateMeterCommand.RealEstateId)));

		dbContext.Meters.Remove(maybeItem);

		await dbContext.SaveChangesAsync(cancellationToken);

		return Result.Success();
	}
}
