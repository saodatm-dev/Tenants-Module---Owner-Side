using Building.Application.Core.Abstractions.Data;
using Building.Application.Core.Abstractions.Services;
using Building.Domain.Complexes;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Complexes.Create;

internal sealed class CreateComplexCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IBuildingDbContext dbContext,
	IGeometryService geometryService) : ICommandHandler<CreateComplexCommand, Guid>
{
	public async Task<Result<Guid>> Handle(CreateComplexCommand command, CancellationToken cancellationToken)
	{
		if (await dbContext.Complexes
			.AsNoTracking()
			.AnyAsync(item => item.Name == command.Name, cancellationToken))
			return Result.Failure<Guid>(sharedViewLocalizer.AlreadyExists(nameof(CreateComplexCommand.Name)));

		if (!await dbContext.RegionTranslates.AsNoTracking().AnyAsync(item => item.RegionId == command.RegionId, cancellationToken))
			return Result.Failure<Guid>(sharedViewLocalizer.NotFound(nameof(CreateComplexCommand.RegionId)));

		if (!await dbContext.DistrictTranslates.AsNoTracking().AnyAsync(item => item.DistrictId == command.DistrictId, cancellationToken))
			return Result.Failure<Guid>(sharedViewLocalizer.NotFound(nameof(CreateComplexCommand.DistrictId)));

		// TODO : Add language validations

		var item = new Complex(
			command.RegionId,
			command.DistrictId,
			command.Name,
			command.Descriptions,
			command.isCommercial,
			command.isLiving,
			command.Latitude != null && command.Longitude != null
				? geometryService.CreatePoint(command.Latitude.Value, command.Longitude.Value)
				: null,
			command.Address,
			command.Images);

		await dbContext.Complexes.AddAsync(item, cancellationToken);

		await dbContext.SaveChangesAsync(cancellationToken);

		return item.Id;
	}
}
