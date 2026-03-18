using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Renovations.Remove;

internal sealed class RemoveRenovationCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IBuildingDbContext dbContext) : ICommandHandler<RemoveRenovationCommand>
{
	public async Task<Result> Handle(RemoveRenovationCommand command, CancellationToken cancellationToken)
	{
		var maybeItem =
			await dbContext.Renovations.FirstOrDefaultAsync(item => item.Id == command.Id, cancellationToken);

		if (maybeItem is null)
			return Result.Failure(sharedViewLocalizer.RenovationNotFound(nameof(RemoveRenovationCommand.Id)));

		dbContext.Renovations.Update(maybeItem.Remove());

		await dbContext.SaveChangesAsync(cancellationToken);

		return Result.Success();
	}
}
