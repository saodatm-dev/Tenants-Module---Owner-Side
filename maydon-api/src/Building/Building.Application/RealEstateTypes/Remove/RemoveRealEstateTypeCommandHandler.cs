using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.RealEstateTypes.Remove;

internal sealed class RemoveRealEstateTypeCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IBuildingDbContext dbContext) : ICommandHandler<RemoveRealEstateTypeCommand>
{
	public async Task<Result> Handle(RemoveRealEstateTypeCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.RealEstateTypes.FirstOrDefaultAsync(item => item.Id == command.Id, cancellationToken);

		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.RealEstateTypeNotFound(nameof(RemoveRealEstateTypeCommand.Id)));

		dbContext.RealEstateTypes.Remove(maybeItem.Remove());

		await dbContext.SaveChangesAsync(cancellationToken);

		return Result.Success();
	}
}
