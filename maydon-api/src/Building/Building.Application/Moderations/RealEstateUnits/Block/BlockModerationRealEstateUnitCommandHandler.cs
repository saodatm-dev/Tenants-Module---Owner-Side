using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Moderations.RealEstateUnits.Block;

internal sealed class BlockModerationRealEstateUnitCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IBuildingDbContext dbContext) : ICommandHandler<BlockModerationRealEstateUnitCommand, Guid>
{
	public async Task<Result<Guid>> Handle(BlockModerationRealEstateUnitCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.Units.FirstOrDefaultAsync(item => item.Id == command.Id, cancellationToken);

		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.RealEstateUnitNotFound(nameof(BlockModerationRealEstateUnitCommand.Id)));

		if (maybeItem.IsBlocked())
			return maybeItem.Id;

		// check status 
		if (maybeItem.IsCancel())
			return Result.Failure<Guid>(sharedViewLocalizer.WasCancelledByUser(nameof(BlockModerationRealEstateUnitCommand.Id)));

		dbContext.Units.Update(maybeItem.Block());

		await dbContext.SaveChangesAsync(cancellationToken);

		return maybeItem.Id;
	}
}
