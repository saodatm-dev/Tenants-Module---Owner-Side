using Building.Application.Core.Abstractions.Data;
using Building.Domain.Floors;
using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Abstractions.Services.Minio;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Floors.Create;

internal sealed class CreateFloorCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IExecutionContextProvider executionContextProvider,
	IBuildingDbContext dbContext,
	IFileManager fileManager) : ICommandHandler<CreateFloorCommand, Guid>
{
	public async Task<Result<Guid>> Handle(CreateFloorCommand command, CancellationToken cancellationToken)
	{
		var hasBuildingId = command.BuildingId is not null && command.BuildingId != Guid.Empty;
		var hasRealEstateId = command.RealEstateId is not null && command.RealEstateId != Guid.Empty;

		if (!hasBuildingId && !hasRealEstateId)
			return Result.Failure<Guid>(sharedViewLocalizer.NotFound("Parent"));

		if (hasBuildingId)
		{
			if (await dbContext.Floors
				.AsNoTracking()
				.AnyAsync(item => item.BuildingId == command.BuildingId && item.Number == command.Number, cancellationToken))
				return Result.Failure<Guid>(sharedViewLocalizer.AlreadyExists(nameof(CreateFloorCommand.Number)));

			if (!await dbContext.Buildings.AsNoTracking().AnyAsync(item => item.Id == command.BuildingId, cancellationToken))
				return Result.Failure<Guid>(sharedViewLocalizer.NotFound(nameof(CreateFloorCommand.BuildingId)));
		}

		if (hasRealEstateId)
		{
			if (await dbContext.Floors
				.AsNoTracking()
				.AnyAsync(item => item.RealEstateId == command.RealEstateId && item.Number == command.Number, cancellationToken))
				return Result.Failure<Guid>(sharedViewLocalizer.AlreadyExists(nameof(CreateFloorCommand.Number)));

			if (!await dbContext.RealEstates.AsNoTracking().AnyAsync(item => item.Id == command.RealEstateId, cancellationToken))
				return Result.Failure<Guid>(sharedViewLocalizer.NotFound(nameof(CreateFloorCommand.RealEstateId)));
		}

		var item = new Floor(
			command.Number,
			command.Type,
			command.Label,
			command.TotalArea,
			command.CeilingHeight,
			realEstateId: command.RealEstateId,
			buildingId: command.BuildingId);

		if (command.Plan is not null)
		{
			item.UpdatePlan(command.Plan);
		}

		await dbContext.Floors.AddAsync(item, cancellationToken);

		await dbContext.SaveChangesAsync(cancellationToken);

		return item.Id;
	}
}
