using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.RentalPurposes.Remove;

internal sealed class RemoveRentalPurposeCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IBuildingDbContext dbContext) : ICommandHandler<RemoveRentalPurposeCommand>
{
	public async Task<Result> Handle(RemoveRentalPurposeCommand command, CancellationToken cancellationToken)
	{
		var maybeItem =
			await dbContext.RentalPurposes.FirstOrDefaultAsync(item => item.Id == command.Id, cancellationToken);

		if (maybeItem is null)
			return Result.Failure(sharedViewLocalizer.NotFound(nameof(RemoveRentalPurposeCommand.Id)));

		dbContext.RentalPurposes.Update(maybeItem.Remove());

		await dbContext.SaveChangesAsync(cancellationToken);

		return Result.Success();
	}
}
