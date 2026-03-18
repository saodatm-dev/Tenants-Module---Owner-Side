using Building.Application.Core.Abstractions.Data;
using Building.Domain.Floors;
using Building.Domain.RealEstates;
using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.RealEstates.Create;

internal sealed class CreateRealEstateCommandHandler(
    ISharedViewLocalizer sharedViewLocalizer,
    IExecutionContextProvider executionContextProvider,
    IBuildingDbContext dbContext) : ICommandHandler<CreateRealEstateCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateRealEstateCommand command, CancellationToken cancellationToken)
    {
        var realEstateTypeExists = await dbContext.RealEstateTypes.AsNoTracking()
            .AnyAsync(item => item.Id == command.RealEstateTypeId, cancellationToken);

        if (!realEstateTypeExists)
            return Result.Failure<Guid>(sharedViewLocalizer.RealEstateTypeNotFound(nameof(CreateRealEstateCommand.RealEstateTypeId)));

        var renovationExists = command.RenovationId is not null && command.RenovationId != Guid.Empty
            ? await dbContext.Renovations.AsNoTracking().AnyAsync(item => item.Id == command.RenovationId, cancellationToken)
            : true;

        if (!renovationExists)
            return Result.Failure<Guid>(sharedViewLocalizer.RenovationNotFound(nameof(CreateRealEstateCommand.RenovationId)));

        var landCategoryExists = command.LandCategoryId is not null && command.LandCategoryId != Guid.Empty
            ? await dbContext.LandCategories.AsNoTracking().AnyAsync(item => item.Id == command.LandCategoryId, cancellationToken)
            : true;

        if (!landCategoryExists)
            return Result.Failure<Guid>(sharedViewLocalizer.LandCategoryNotFound(nameof(CreateRealEstateCommand.LandCategoryId)));

        var productionTypeExists = command.ProductionTypeId is not null && command.ProductionTypeId != Guid.Empty
            ? await dbContext.ProductionTypes.AsNoTracking().AnyAsync(item => item.Id == command.ProductionTypeId, cancellationToken)
            : true;

        if (!productionTypeExists)
            return Result.Failure<Guid>(sharedViewLocalizer.ProductionTypeNotFound(nameof(CreateRealEstateCommand.ProductionTypeId)));

        var regionExists = command.RegionId is not null && command.RegionId != Guid.Empty
            ? await dbContext.RegionTranslates.AsNoTracking().AnyAsync(item => item.RegionId == command.RegionId, cancellationToken)
            : true;

        if (!regionExists)
            return Result.Failure<Guid>(sharedViewLocalizer.RegionNotFound(nameof(CreateRealEstateCommand.RegionId)));

        var districtExists = command.DistrictId is not null && command.DistrictId != Guid.Empty
            ? await dbContext.DistrictTranslates.AsNoTracking().AnyAsync(item => item.DistrictId == command.DistrictId, cancellationToken)
            : true;

        if (!districtExists)
            return Result.Failure<Guid>(sharedViewLocalizer.DistrictNotFound(nameof(CreateRealEstateCommand.DistrictId)));

        Floor? floor = null;
        Domain.Buildings.Building? building = null;

        if (command.FloorId is not null && command.FloorId != Guid.Empty)
        {
            floor = await dbContext.Floors
                .Include(item => item.Building)
                .FirstOrDefaultAsync(item => item.Id == command.FloorId, cancellationToken);
        }
        else if (command.BuildingId is not null && command.BuildingId != Guid.Empty)
        {
            building = await dbContext.Buildings
                .FirstOrDefaultAsync(item => item.Id == command.BuildingId, cancellationToken);
        }

        if (floor is not null)
            building = floor.Building;

        var item = new RealEstate(
            executionContextProvider.TenantId,
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
            command.Latitude != null && command.Longitude != null
                ? new NetTopologySuite.Geometries.Point(command.Latitude.Value, command.Longitude.Value)
                : null,
            command.Address,
            command.plan,
            images: command.Images,
            amenityIds: command.AmenityIds);

        await dbContext.RealEstates.AddAsync(item, cancellationToken);

        var aboveCount = command.AboveFloors ?? 0;
        var belowCount = command.BelowFloors ?? 0;
        var hasAboveOrBelow = aboveCount > 0 || belowCount > 0;
        var totalToCreate = hasAboveOrBelow
            ? aboveCount + belowCount
            : (command.TotalFloors ?? 0);

        if (totalToCreate > 0)
        {
            var floors = new List<Floor>(totalToCreate);

            if (hasAboveOrBelow)
            {
                for (short i = (short)-belowCount; i <= -1; i++)
                {
                    floors.Add(new Floor(
                        number: i,
                        type: FloorType.Basement,
                        realEstateId: item.Id));
                }

                for (short i = 1; i <= aboveCount; i++)
                {
                    floors.Add(new Floor(
                        number: i,
                        type: FloorType.Regular,
                        realEstateId: item.Id));
                }
            }
            else
            {
                for (short i = 1; i <= command.TotalFloors; i++)
                {
                    floors.Add(new Floor(
                        number: i,
                        type: FloorType.Regular,
                        realEstateId: item.Id));
                }
            }

            await dbContext.Floors.AddRangeAsync(floors, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return item.Id;
    }
}
