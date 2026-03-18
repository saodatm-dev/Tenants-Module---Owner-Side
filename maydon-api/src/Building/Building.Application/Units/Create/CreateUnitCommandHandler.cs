using Building.Application.Core.Abstractions.Data;
using Building.Domain.Floors;
using Building.Domain.Rooms;
using Building.Domain.Units;
using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Units.Create;

internal sealed class CreateUnitCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IExecutionContextProvider executionContextProvider,
	IBuildingDbContext dbContext) : ICommandHandler<CreateUnitCommand, Guid>
{
	public async Task<Result<Guid>> Handle(CreateUnitCommand command, CancellationToken cancellationToken)
	{
		if (command.RealEstateTypeId is not null && !await dbContext.RealEstates.AsNoTracking().AnyAsync(item => item.Id == command.RealEstateId, cancellationToken))
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

		var unit = new Unit(
			executionContextProvider.TenantId,
			command.RealEstateId,
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
			command.Images);

		await dbContext.Units.AddAsync(unit, cancellationToken);

		await dbContext.SaveChangesAsync(cancellationToken);

		return unit.Id;
	}
}
