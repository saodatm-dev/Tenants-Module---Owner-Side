using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Moderations.RealEstateUnits.Reject;

internal sealed class RejectModerationRealEstateUnitCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IBuildingDbContext dbContext) : ICommandHandler<RejectModerationRealEstateUnitCommand, Guid>
{
	public async Task<Result<Guid>> Handle(RejectModerationRealEstateUnitCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.Units.FirstOrDefaultAsync(item => item.Id == command.Id, cancellationToken);

		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.RealEstateUnitNotFound(nameof(RejectModerationRealEstateUnitCommand.Id)));

		if (maybeItem.IsReject())
			return maybeItem.Id;

		// check status 
		if (maybeItem.IsCancel())
			return Result.Failure<Guid>(sharedViewLocalizer.WasCancelledByUser(nameof(RejectModerationRealEstateUnitCommand.Id)));

		dbContext.Units.Update(maybeItem.Reject(command.Reason));

		await dbContext.SaveChangesAsync(cancellationToken);

		return maybeItem.Id;
	}
}
