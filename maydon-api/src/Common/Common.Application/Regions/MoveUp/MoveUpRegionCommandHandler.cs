using Common.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Common.Application.Regions.MoveUp;

internal sealed class MoveUpRegionCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	ICommonDbContext dbContext) : ICommandHandler<MoveUpRegionCommand>
{
	public async Task<Result> Handle(MoveUpRegionCommand command, CancellationToken cancellationToken)
	{
		var currentItem = await dbContext.Regions.FirstOrDefaultAsync(item => item.Id == command.Id, cancellationToken);
		if (currentItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.RegionNotFound(nameof(MoveUpRegionCommand.Id)));

		var previousItem = await dbContext.Regions
			.OrderBy(item => item.Order)
			.LastOrDefaultAsync(item => item.Order < currentItem.Order, cancellationToken);

		if (previousItem is null)
			return Result.Success();

		var currentItemOrder = currentItem.Order;
		dbContext.Regions.Update(currentItem.SetOrder(previousItem.Order));
		dbContext.Regions.Update(previousItem.SetOrder(currentItemOrder));

		await dbContext.SaveChangesAsync(cancellationToken);

		return Result.Success();
	}
}
