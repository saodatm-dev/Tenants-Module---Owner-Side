using Building.Application.Core.Abstractions.Data;
using Building.Application.MeterTypes.Update;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.MeterTypes.Remove;

internal sealed class RemoveMeterTypeCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IBuildingDbContext dbContext) : ICommandHandler<RemoveMeterTypeCommand, Guid>
{
	public async Task<Result<Guid>> Handle(RemoveMeterTypeCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.MeterTypes.FirstOrDefaultAsync(item => item.Id == command.Id, cancellationToken);
		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.NotFound(nameof(UpdateMeterTypeCommand.Names)));

		dbContext.MeterTypes.Remove(maybeItem.Remove());

		await dbContext.SaveChangesAsync(cancellationToken);

		return maybeItem.Id;
	}
}
