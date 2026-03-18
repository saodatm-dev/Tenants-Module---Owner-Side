using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.LandCategories.Remove;

internal sealed class RemoveLandCategoryCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IBuildingDbContext dbContext) : ICommandHandler<RemoveLandCategoryCommand>
{
	public async Task<Result> Handle(RemoveLandCategoryCommand command, CancellationToken cancellationToken)
	{
		var maybeItem =
			await dbContext.LandCategories.FirstOrDefaultAsync(item => item.Id == command.Id, cancellationToken);

		if (maybeItem is null)
			return Result.Failure(sharedViewLocalizer.LandCategoryNotFound(nameof(RemoveLandCategoryCommand.Id)));

		dbContext.LandCategories.Update(maybeItem.Remove());

		await dbContext.SaveChangesAsync(cancellationToken);

		return Result.Success();
	}
}
