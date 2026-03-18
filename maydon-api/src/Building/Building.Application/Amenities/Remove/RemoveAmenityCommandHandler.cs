using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;

namespace Building.Application.Amenities.Remove;

internal sealed class RemoveAmenityCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IBuildingDbContext dbContext) : ICommandHandler<RemoveAmenityCommand>
{
	public async Task<Result> Handle(RemoveAmenityCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.Amenities.FindAsync([command.Id], cancellationToken);
		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.NotFound(nameof(RemoveAmenityCommand.Id)));

		dbContext.Amenities.Remove(maybeItem.Remove());

		await dbContext.SaveChangesAsync(cancellationToken);

		return Result.Success();
	}
}
