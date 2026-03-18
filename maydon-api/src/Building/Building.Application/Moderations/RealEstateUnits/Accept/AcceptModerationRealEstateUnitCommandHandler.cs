using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Moderations.RealEstateUnits.Accept;

internal sealed class AcceptModerationRealEstateUnitCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IBuildingDbContext dbContext) : ICommandHandler<AcceptModerationRealEstateUnitCommand, Guid>
{
	public async Task<Result<Guid>> Handle(AcceptModerationRealEstateUnitCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.Units.FirstOrDefaultAsync(item => item.Id == command.Id, cancellationToken);

		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.RealEstateUnitNotFound(nameof(AcceptModerationRealEstateUnitCommand.Id)));

		if (maybeItem.IsAccept())
			return maybeItem.Id;

		// check status 
		if (maybeItem.IsCancel())
			return Result.Failure<Guid>(sharedViewLocalizer.WasCancelledByUser(nameof(AcceptModerationRealEstateUnitCommand.Id)));

		dbContext.Units.Update(maybeItem.Accept());

		await dbContext.SaveChangesAsync(cancellationToken);

		return maybeItem.Id;
	}
}
