using Building.Application.Core.Abstractions.Data;
using Building.Domain.Floors;
using Building.Domain.Rooms;
using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Rooms.Create;

internal sealed class CreateRoomCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IExecutionContextProvider executionContextProvider,
	IBuildingDbContext dbContext) : ICommandHandler<CreateRoomCommand, Guid>
{
	public async Task<Result<Guid>> Handle(CreateRoomCommand command, CancellationToken cancellationToken)
	{
		if (!await dbContext.RealEstates
			.AsNoTracking()
			.AnyAsync(item => item.Id == command.RealEstateId && item.OwnerId == executionContextProvider.TenantId, cancellationToken))
			return Result.Failure<Guid>(sharedViewLocalizer.RealEstateNotFound(nameof(CreateRoomCommand.RealEstateId)));

		if (!await dbContext.RoomTypes
			.AsNoTracking()
			.AnyAsync(item => item.Id == command.RoomTypeId, cancellationToken))
			return Result.Failure<Guid>(sharedViewLocalizer.RoomTypeNotFound(nameof(CreateRoomCommand.RoomTypeId)));

		Floor floor = null;
		if (command.FloorId is not null)
		{
			floor = await dbContext.Floors.AsNoTracking().FirstOrDefaultAsync(item => item.Id == command.FloorId, cancellationToken);
			if (floor is null)
				return Result.Failure<Guid>(sharedViewLocalizer.FloorNotFound(nameof(CreateRoomCommand.FloorId)));
		}

		var item = new Room(
			command.RealEstateId,
			command.RoomTypeId,
			floor,
			command.Number,
			command.TotalArea);

		await dbContext.Rooms.AddAsync(item, cancellationToken);

		await dbContext.SaveChangesAsync(cancellationToken);

		return item.Id;
	}
}
