using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Categories.Remove;

internal sealed class RemoveCategoryCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IBuildingDbContext dbContext) : ICommandHandler<RemoveCategoryCommand>
{
	public async Task<Result> Handle(RemoveCategoryCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.Categories.FirstOrDefaultAsync(item => item.Id == command.Id, cancellationToken);
		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.NotFound(nameof(RemoveCategoryCommand.Id)));

		dbContext.Categories.Remove(maybeItem.Remove());

		await dbContext.SaveChangesAsync(cancellationToken);

		return Result.Success();
	}
}
