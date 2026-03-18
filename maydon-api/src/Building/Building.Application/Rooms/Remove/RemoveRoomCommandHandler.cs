using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Rooms.Remove;

internal sealed class RemoveRoomCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IExecutionContextProvider executionContextProvider,
	IBuildingDbContext dbContext) : ICommandHandler<RemoveRoomCommand>
{
	public async Task<Result> Handle(RemoveRoomCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.Rooms
			.Include(item => item.RealEstate)
			.FirstOrDefaultAsync(item => item.Id == command.Id && item.RealEstate.OwnerId == executionContextProvider.TenantId, cancellationToken);

		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.RoomNotFound(nameof(RemoveRoomCommand.Id)));

		dbContext.Rooms.Remove(maybeItem);

		await dbContext.SaveChangesAsync(cancellationToken);

		return Result.Success();
	}
}
