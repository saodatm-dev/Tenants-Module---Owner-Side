using Building.Application.Core.Abstractions.Data;
using Building.Domain.Complexes;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Building.Application.Buildings.Create;

internal sealed class CreateBuildingCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IBuildingDbContext dbContext) : ICommandHandler<CreateBuildingCommand, Guid>
{
	public async Task<Result<Guid>> Handle(CreateBuildingCommand command, CancellationToken cancellationToken)
	{
		if (command.RegionId != null && !await dbContext.RegionTranslates.AsNoTracking().AnyAsync(item => item.RegionId == command.RegionId, cancellationToken))
			return Result.Failure<Guid>(sharedViewLocalizer.NotFound(nameof(CreateBuildingCommand.RegionId)));

		if (command.DistrictId != null && !await dbContext.DistrictTranslates.AsNoTracking().AnyAsync(item => item.DistrictId == command.DistrictId, cancellationToken))
			return Result.Failure<Guid>(sharedViewLocalizer.NotFound(nameof(CreateBuildingCommand.DistrictId)));

		Complex complex = null;
		if (command.ComplexId != null && command.ComplexId != Guid.Empty)
		{
			complex = await dbContext.Complexes.AsNoTracking().FirstOrDefaultAsync(item => item.Id == command.ComplexId, cancellationToken);
			if (complex is null)
				return Result.Failure<Guid>(sharedViewLocalizer.NotFound(nameof(CreateBuildingCommand.ComplexId)));

			if (await dbContext.Buildings.AsNoTracking().AnyAsync(item => item.ComplexId == command.ComplexId && item.Number == command.Number, cancellationToken))
				return Result.Failure<Guid>(sharedViewLocalizer.AlreadyExists(nameof(CreateBuildingCommand.Number)));
		}

		// TODO : Add language validations

		var item = new Building.Domain.Buildings.Building(
			command.RegionId,
			command.DistrictId,
			command.Number,
			command.IsCommercial,
			command.IsLiving,
			command.TotalArea,
			command.FloorsCount,
			complex,
			command.Latitude != null && command.Longitude != null
				? new Point(command.Latitude.Value, command.Longitude.Value)
				: null,
			command.Address,
			command.Descriptions,
			command.Images);

		await dbContext.Buildings.AddAsync(item, cancellationToken);

		await dbContext.SaveChangesAsync(cancellationToken);

		return item.Id;
	}
}
