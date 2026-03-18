using Building.Application.Core.Abstractions.Data;
using Building.Domain.Floors;
using Building.Domain.RealEstates;
using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Building.Application.RealEstates.Update;

internal sealed class UpdateRealEstateCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IExecutionContextProvider executionContextProvider,
	IBuildingDbContext dbContext) : ICommandHandler<UpdateRealEstateCommand, Guid>
{
	public async Task<Result<Guid>> Handle(UpdateRealEstateCommand command, CancellationToken cancellationToken)
	{
		// check type
		if (!await dbContext.RealEstateTypes.AsNoTracking().AnyAsync(item => item.Id != command.RealEstateTypeId, cancellationToken))
			return Result.Failure<Guid>(sharedViewLocalizer.RealEstateTypeNotFound(nameof(UpdateRealEstateCommand.RealEstateTypeId)));

		var maybeItem = await dbContext.RealEstates
			.IsUpdatable(executionContextProvider.TenantId)
			.FirstOrDefaultAsync(item => item.Id == command.Id && item.OwnerId == executionContextProvider.TenantId, cancellationToken);

		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.RealEstateNotFound(nameof(UpdateRealEstateCommand.Id)));

		Floor floor = null;
		Domain.Buildings.Building building = null;

		if (command.FloorId is not null && command.FloorId != Guid.Empty)
		{
			floor = await dbContext.Floors
				.AsNoTracking()
				.Include(item => item.Building)
				.FirstOrDefaultAsync(item => item.Id == command.FloorId, cancellationToken);

			if (floor is not null)
				building = floor.Building;
		}
		else if (command.BuildingId is not null && command.BuildingId != Guid.Empty)
		{
			building = await dbContext.Buildings
				.AsNoTracking()
				.FirstOrDefaultAsync(item => item.Id == command.FloorId, cancellationToken);
		}

		dbContext.RealEstates.Update(
			maybeItem.Update(
				command.RealEstateTypeId,
				command.BuildingNumber,
				command.FloorNumber,
				command.Number,
				command.RenovationId,
				command.LandCategoryId,
				command.ProductionTypeId,
				command.CadastralNumber,
				command.TotalArea,
				command.LivingArea,
				command.CeilingHeight,
				command.TotalFloors,
				command.AboveFloors,
				command.BelowFloors,
				command.RoomsCount,
				building,
				floor,
				command.Units,
				command.Rooms,
				command.RegionId,
				command.DistrictId,
				command.Latitude != null && command.Longitude != null ? new Point(command.Latitude.Value, command.Longitude.Value) : null,
				command.Address,
				command.plan,
				amenityIds: command.AmenityIds));

		await dbContext.SaveChangesAsync(cancellationToken);

		return maybeItem.Id;
	}
}
