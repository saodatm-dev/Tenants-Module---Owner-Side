using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;

namespace Building.Application.AmenityCategories.Remove;

internal sealed class RemoveAmenityCategoryCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IBuildingDbContext dbContext) : ICommandHandler<RemoveAmenityCategoryCommand>
{
	public async Task<Result> Handle(RemoveAmenityCategoryCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.AmenityCategories.FindAsync([command.Id], cancellationToken);
		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.NotFound(nameof(RemoveAmenityCategoryCommand.Id)));

		dbContext.AmenityCategories.Remove(maybeItem.Remove());

		await dbContext.SaveChangesAsync(cancellationToken);

		return Result.Success();
	}
}
