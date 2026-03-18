using Common.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Common.Application.Regions.Remove;

internal sealed class RemoveRegionCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	ICommonDbContext dbContext) : ICommandHandler<RemoveRegionCommand>
{
	public async Task<Result> Handle(RemoveRegionCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.Regions.FirstOrDefaultAsync(item => item.Id == command.Id, cancellationToken);
		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.RegionNotFound(nameof(RemoveRegionCommand.Id)));

		dbContext.Regions.Remove(maybeItem.Remove());

		await dbContext.SaveChangesAsync(cancellationToken);

		return Result.Success();
	}
}
