using Building.Application.Core.Abstractions.Data;
using Building.Domain.RealEstates;
using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.RealEstates.Remove;

internal sealed class RemoveRealEstateCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IExecutionContextProvider executionContextProvider,
	IBuildingDbContext dbContext) : ICommandHandler<RemoveRealEstateCommand>
{
	public async Task<Result> Handle(RemoveRealEstateCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.RealEstates
			.IsUpdatable(executionContextProvider.TenantId)
			.FirstOrDefaultAsync(item => item.Id == command.Id, cancellationToken);

		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.RealEstateNotFound(nameof(RemoveRealEstateCommand.Id)));

		dbContext.RealEstates.Remove(maybeItem.Remove());

		await dbContext.SaveChangesAsync(cancellationToken);

		return Result.Success();
	}
}
