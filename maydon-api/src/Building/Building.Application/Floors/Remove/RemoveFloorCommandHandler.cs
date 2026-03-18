using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Floors.Remove;

internal sealed class RemoveFloorCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IBuildingDbContext dbContext) : ICommandHandler<RemoveFloorCommand>
{
	public async Task<Result> Handle(RemoveFloorCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.Floors
			.FirstOrDefaultAsync(item => item.Id == command.Id, cancellationToken);

		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.NotFound(nameof(RemoveFloorCommand.Id)));

		dbContext.Floors.Remove(maybeItem);

		await dbContext.SaveChangesAsync(cancellationToken);

		return Result.Success();
	}
}
