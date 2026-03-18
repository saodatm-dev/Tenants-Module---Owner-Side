using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Floors.Update;

internal sealed class UpdateFloorCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IBuildingDbContext dbContext) : ICommandHandler<UpdateFloorCommand, Guid>
{
	public async Task<Result<Guid>> Handle(UpdateFloorCommand command, CancellationToken cancellationToken)
	{
		var hasBuildingId = command.BuildingId is not null && command.BuildingId != Guid.Empty;
		var hasRealEstateId = command.RealEstateId is not null && command.RealEstateId != Guid.Empty;

		if (!hasBuildingId && !hasRealEstateId)
			return Result.Failure<Guid>(sharedViewLocalizer.NotFound("Parent"));

		if (hasBuildingId)
		{
			if (!await dbContext.Buildings.AsNoTracking().AnyAsync(item => item.Id == command.BuildingId, cancellationToken))
				return Result.Failure<Guid>(sharedViewLocalizer.NotFound(nameof(UpdateFloorCommand.BuildingId)));
		}

		if (hasRealEstateId)
		{
			if (!await dbContext.RealEstates.AsNoTracking().AnyAsync(item => item.Id == command.RealEstateId, cancellationToken))
				return Result.Failure<Guid>(sharedViewLocalizer.NotFound(nameof(UpdateFloorCommand.RealEstateId)));
		}

		var maybeItem = await dbContext.Floors
			.FirstOrDefaultAsync(item => item.Id == command.Id, cancellationToken);

		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.NotFound(nameof(UpdateFloorCommand.Id)));

		dbContext.Floors.Update(
			maybeItem.Update(
				command.Number,
				command.Type,
				command.Label,
				command.TotalArea,
				command.CeilingHeight,
				command.FloorPlan,
				command.BuildingId,
				command.RealEstateId));

		await dbContext.SaveChangesAsync(cancellationToken);

		return maybeItem.Id;
	}
}
