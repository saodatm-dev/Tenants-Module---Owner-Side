using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Moderations.RealEstateUnits.Cancel;

internal sealed class CancelModerationRealEstateUnitCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IBuildingDbContext dbContext) : ICommandHandler<CancelModerationRealEstateUnitCommand, Guid>
{
	public async Task<Result<Guid>> Handle(CancelModerationRealEstateUnitCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.Units.FirstOrDefaultAsync(item => item.Id == command.Id, cancellationToken);

		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.RealEstateUnitNotFound(nameof(CancelModerationRealEstateUnitCommand.Id)));

		if (maybeItem.IsCancel())
			return maybeItem.Id;

		// check status 
		if (maybeItem.IsBlocked())
			return Result.Failure<Guid>(sharedViewLocalizer.WasBlockedByModerator(nameof(CancelModerationRealEstateUnitCommand.Id)));

		if (maybeItem.IsReject())
			return Result.Failure<Guid>(sharedViewLocalizer.WasRejectedByModerator(nameof(CancelModerationRealEstateUnitCommand.Id)));

		if (maybeItem.IsAccept())
			return Result.Failure<Guid>(sharedViewLocalizer.WasAcceptedByModerator(nameof(CancelModerationRealEstateUnitCommand.Id)));

		dbContext.Units.Update(maybeItem.Cancel());

		await dbContext.SaveChangesAsync(cancellationToken);

		return maybeItem.Id;
	}
}
