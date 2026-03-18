using Common.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Common.Application.Districts.MoveDown;

internal sealed class MoveDownDistrictCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	ICommonDbContext dbContext) : ICommandHandler<MoveDownDistrictCommand>
{
	public async Task<Result> Handle(MoveDownDistrictCommand command, CancellationToken cancellationToken)
	{
		var currentItem = await dbContext.Districts.FirstOrDefaultAsync(item => item.Id == command.Id, cancellationToken);
		if (currentItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.DistrictNotFound(nameof(MoveDownDistrictCommand.Id)));

		var nextItem = await dbContext.Districts
			.OrderBy(item => item.Order)
			.FirstOrDefaultAsync(item => item.Order > currentItem.Order, cancellationToken);

		if (nextItem is null)
			return Result.Success();

		// swap
		var currentItemOrder = currentItem.Order;
		dbContext.Districts.Update(currentItem.SetOrder(nextItem.Order));
		dbContext.Districts.Update(nextItem.SetOrder(currentItemOrder));

		await dbContext.SaveChangesAsync(cancellationToken);

		return Result.Success();
	}
}
