using Building.Application.Core.Abstractions.Data;
using Building.Application.Units.Create;
using Building.Domain.Floors;
using Building.Domain.Rooms;
using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Units.Update;

internal sealed class UpdateUnitCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IExecutionContextProvider executionContextProvider,
	IBuildingDbContext dbContext) : ICommandHandler<UpdateUnitCommand, Guid>
{
	public async Task<Result<Guid>> Handle(UpdateUnitCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.Units
			.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.NotFound(nameof(UpdateUnitCommand.Id)));

		if (maybeItem.OwnerId != executionContextProvider.TenantId)
			return Result.Failure<Guid>(sharedViewLocalizer.NotFound(nameof(UpdateUnitCommand.Id)));

		if (command.RealEstateId is not null && !await dbContext.RealEstates.AsNoTracking().AnyAsync(item => item.Id == command.RealEstateTypeId, cancellationToken))
			return Result.Failure<Guid>(sharedViewLocalizer.RealEstateTypeNotFound(nameof(CreateUnitCommand.RealEstateId)));

		if (command.RealEstateTypeId is not null && !await dbContext.RealEstateTypes.AsNoTracking().AnyAsync(item => item.Id == command.RealEstateTypeId, cancellationToken))
			return Result.Failure<Guid>(sharedViewLocalizer.RealEstateTypeNotFound(nameof(CreateUnitCommand.RealEstateTypeId)));

		Floor floor = null;
		if (command.FloorId is not null)
		{
			floor = await dbContext.Floors.AsNoTracking().FirstOrDefaultAsync(item => item.Id == command.FloorId, cancellationToken);
			if (floor is null)
				return Result.Failure<Guid>(sharedViewLocalizer.FloorNotFound(nameof(CreateUnitCommand.FloorId)));
		}

		Room room = null;
		if (command.RenovationId is not null)
		{
			room = await dbContext.Rooms.AsNoTracking().FirstOrDefaultAsync(item => item.Id == command.RenovationId, cancellationToken);
			if (room is null)
				return Result.Failure<Guid>(sharedViewLocalizer.RoomNotFound(nameof(CreateUnitCommand.RoomId)));
		}

		if (command.RenovationId is not null)
		{
			if (!await dbContext.Renovations.AsNoTracking().AnyAsync(item => item.Id == command.RenovationId, cancellationToken))
				return Result.Failure<Guid>(sharedViewLocalizer.RenovationNotFound(nameof(CreateUnitCommand.RenovationId)));
		}

		dbContext.Units.Update(
			maybeItem.Update(
				command.RealEstateTypeId,
				floor,
				room,
				command.FloorNumber,
				command.RoomNumber,
				command.RenovationId,
				command.TotalArea,
				command.CeilingHeight,
				command.Plan,
				command.Coordinates,
				command.Images));

		await dbContext.SaveChangesAsync(cancellationToken);

		return maybeItem.Id;
	}
}
