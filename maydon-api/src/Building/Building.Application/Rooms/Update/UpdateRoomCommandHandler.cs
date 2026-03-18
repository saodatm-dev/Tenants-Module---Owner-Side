using Building.Application.Core.Abstractions.Data;
using Building.Domain.Floors;
using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Rooms.Update;

internal sealed class UpdateRoomCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IExecutionContextProvider executionContextProvider,
	IBuildingDbContext dbContext) : ICommandHandler<UpdateRoomCommand, Guid>
{
	public async Task<Result<Guid>> Handle(UpdateRoomCommand command, CancellationToken cancellationToken)
	{
		if (!await dbContext.RealEstates
			.AsNoTracking()
			.AnyAsync(item => item.Id == command.RealEstateId && item.OwnerId == executionContextProvider.TenantId, cancellationToken))
			return Result.Failure<Guid>(sharedViewLocalizer.RealEstateNotFound(nameof(UpdateRoomCommand.RealEstateId)));

		if (!await dbContext.RoomTypes
			.AsNoTracking()
			.AnyAsync(item => item.Id == command.RoomTypeId, cancellationToken))
			return Result.Failure<Guid>(sharedViewLocalizer.RoomTypeNotFound(nameof(UpdateRoomCommand.RoomTypeId)));

		Floor floor = null;
		if (command.FloorId is not null)
		{
			floor = await dbContext.Floors.AsNoTracking().FirstOrDefaultAsync(item => item.Id == command.FloorId, cancellationToken);
			if (floor is null)
				return Result.Failure<Guid>(sharedViewLocalizer.FloorNotFound(nameof(UpdateRoomCommand.FloorId)));
		}

		var maybeItem = await dbContext.Rooms
			.Include(item => item.RealEstate)
			.FirstOrDefaultAsync(item => item.Id == command.Id && item.RealEstate.OwnerId == executionContextProvider.TenantId, cancellationToken);

		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.RoomNotFound(nameof(UpdateRoomCommand.Id)));

		dbContext.Rooms.Update(
			maybeItem.Update(
				command.RealEstateId,
				command.RoomTypeId,
				floor,
				command.Number,
				command.TotalArea));

		await dbContext.SaveChangesAsync(cancellationToken);

		return maybeItem.Id;
	}
}
