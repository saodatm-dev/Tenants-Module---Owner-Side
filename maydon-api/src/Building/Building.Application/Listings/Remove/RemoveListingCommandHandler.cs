using Building.Application.Core.Abstractions.Data;
using Building.Application.RealEstates.Remove;
using Building.Domain.Listings;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Listings.Remove;

internal sealed class RemoveListingCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IBuildingDbContext dbContext) : ICommandHandler<RemoveListingCommand>
{
	public async Task<Result> Handle(RemoveListingCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.Listings
			.IsUpdatable()
			.FirstOrDefaultAsync(item => item.Id == command.Id, cancellationToken);

		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.ListingNotFound(nameof(RemoveRealEstateCommand.Id)));

		dbContext.Listings.Remove(maybeItem.Remove());

		await dbContext.SaveChangesAsync(cancellationToken);

		return Result.Success();
	}
}
