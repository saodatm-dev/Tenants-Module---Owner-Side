using Common.Application.Core.Abstractions.Data;
using Common.Application.Districts.MoveUp;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Application.Districts.MoveUp;

internal sealed class MoveUpDistrictCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	ICommonDbContext dbContext) : ICommandHandler<MoveUpDistrictCommand>
{
	public async Task<Result> Handle(MoveUpDistrictCommand command, CancellationToken cancellationToken)
	{
		var currentItem = await dbContext.Districts.FirstOrDefaultAsync(item => item.Id == command.Id, cancellationToken);
		if (currentItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.DistrictNotFound(nameof(MoveUpDistrictCommand.Id)));

		var previousItem = await dbContext.Districts
			.OrderBy(item => item.Order)
			.LastOrDefaultAsync(item => item.Order < currentItem.Order, cancellationToken);

		if (previousItem is null)
			return Result.Success();

		var currentItemOrder = currentItem.Order;
		dbContext.Districts.Update(currentItem.SetOrder(previousItem.Order));
		dbContext.Districts.Update(previousItem.SetOrder(currentItemOrder));

		await dbContext.SaveChangesAsync(cancellationToken);

		return Result.Success();
	}
}
