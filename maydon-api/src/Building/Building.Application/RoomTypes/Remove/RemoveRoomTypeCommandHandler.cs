using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.RoomTypes.Remove;

internal sealed class RemoveRoomTypeCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IBuildingDbContext dbContext) : ICommandHandler<RemoveRoomTypeCommand>
{
	public async Task<Result> Handle(RemoveRoomTypeCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.RoomTypes.FirstOrDefaultAsync(item => item.Id == command.Id, cancellationToken);
		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.RoomTypeNotFound(nameof(RemoveRoomTypeCommand.Id)));

		dbContext.RoomTypes.Remove(maybeItem.Remove());

		await dbContext.SaveChangesAsync(cancellationToken);

		return Result.Success();
	}
}
