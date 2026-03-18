using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Wishlists.Remove;

internal sealed class RemoveWishlistCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IBuildingDbContext dbContext) : ICommandHandler<RemoveWishlistCommand>
{
	public async Task<Result> Handle(RemoveWishlistCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.Wishlists.FirstOrDefaultAsync(item => item.ListringId == command.ListingId, cancellationToken);

		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.WishlistNotFound(nameof(RemoveWishlistCommand.ListingId)));

		dbContext.Wishlists.Remove(maybeItem);

		await dbContext.SaveChangesAsync(cancellationToken);

		return Result.Success();
	}
}
