using Common.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Common.Application.Regions.MoveDown;

internal sealed class MoveDownRegionCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	ICommonDbContext dbContext) : ICommandHandler<MoveDownRegionCommand>
{
	public async Task<Result> Handle(MoveDownRegionCommand command, CancellationToken cancellationToken)
	{
		var currentItem = await dbContext.Regions.FirstOrDefaultAsync(item => item.Id == command.Id, cancellationToken);
		if (currentItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.RegionNotFound(nameof(MoveDownRegionCommand.Id)));

		var nextItem = await dbContext.Regions
			.OrderBy(item => item.Order)
			.FirstOrDefaultAsync(item => item.Order > currentItem.Order, cancellationToken);

		if (nextItem is null)
			return Result.Success();

		// swap
		var currentItemOrder = currentItem.Order;
		dbContext.Regions.Update(currentItem.SetOrder(nextItem.Order));
		dbContext.Regions.Update(nextItem.SetOrder(currentItemOrder));

		await dbContext.SaveChangesAsync(cancellationToken);

		return Result.Success();
	}
}
