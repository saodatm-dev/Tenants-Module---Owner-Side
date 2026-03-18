using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.ProductionTypes.Remove;

internal sealed class RemoveProductionTypeCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IBuildingDbContext dbContext) : ICommandHandler<RemoveProductionTypeCommand>
{
	public async Task<Result> Handle(RemoveProductionTypeCommand command, CancellationToken cancellationToken)
	{
		var maybeItem =
			await dbContext.ProductionTypes.FirstOrDefaultAsync(item => item.Id == command.Id, cancellationToken);

		if (maybeItem is null)
			return Result.Failure(sharedViewLocalizer.ProductionTypeNotFound(nameof(RemoveProductionTypeCommand.Id)));

		dbContext.ProductionTypes.Update(maybeItem.Remove());

		await dbContext.SaveChangesAsync(cancellationToken);

		return Result.Success();
	}
}
