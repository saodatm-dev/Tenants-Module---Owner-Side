using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Units.Remove;

internal sealed class RemoveUnitCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IExecutionContextProvider executionContextProvider,
	IBuildingDbContext dbContext) : ICommandHandler<RemoveUnitCommand>
{
	public async Task<Result> Handle(RemoveUnitCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.Units
			.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

		if (maybeItem is null)
			return Result.Failure(sharedViewLocalizer.NotFound(nameof(RemoveUnitCommand.Id)));

		if (maybeItem.OwnerId != executionContextProvider.TenantId)
			return Result.Failure(sharedViewLocalizer.NotFound(nameof(RemoveUnitCommand.Id)));

		dbContext.Units.Remove(maybeItem.Remove());

		await dbContext.SaveChangesAsync(cancellationToken);

		return Result.Success();
	}
}
