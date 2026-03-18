using Common.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;

namespace Common.Application.Districts.Remove;

internal sealed class RemoveDistrictCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	ICommonDbContext dbContext) : ICommandHandler<RemoveDistrictCommand>
{
	public async Task<Result> Handle(RemoveDistrictCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.Districts.FindAsync(new object?[] { command.Id }, cancellationToken);
		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.RegionNotFound(nameof(RemoveDistrictCommand.Id)));

		dbContext.Districts.Remove(maybeItem.Remove());

		await dbContext.SaveChangesAsync(cancellationToken);

		return Result.Success();
	}
}
