using Building.Application.Core.Abstractions.Data;
using Building.Application.Core.Abstractions.Services;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Complexes.Update;

internal sealed class UpdateComplexCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IBuildingDbContext dbContext,
	IGeometryService geometryService) : ICommandHandler<UpdateComplexCommand, Guid>
{
	public async Task<Result<Guid>> Handle(UpdateComplexCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.Complexes.FirstOrDefaultAsync(item => item.Id == command.Id, cancellationToken);
		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.NotFound(nameof(UpdateComplexCommand.Id)));

		if (!await dbContext.RegionTranslates.AsNoTracking().AnyAsync(item => item.RegionId == command.RegionId, cancellationToken))
			return Result.Failure<Guid>(sharedViewLocalizer.NotFound(nameof(UpdateComplexCommand.RegionId)));

		if (!await dbContext.DistrictTranslates.AsNoTracking().AnyAsync(item => item.DistrictId == command.DistrictId, cancellationToken))
			return Result.Failure<Guid>(sharedViewLocalizer.NotFound(nameof(UpdateComplexCommand.DistrictId)));

		// TODO : Add language validations

		dbContext.Complexes.Update(
			maybeItem.Update(
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
				command.Images));

		await dbContext.SaveChangesAsync(cancellationToken);

		return maybeItem.Id;
	}
}
