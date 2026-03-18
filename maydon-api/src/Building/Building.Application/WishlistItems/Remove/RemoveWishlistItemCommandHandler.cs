using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.WishlistItems.Remove;

internal sealed class RemoveWishlistItemCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IExecutionContextProvider executionContextProvider,
	IBuildingDbContext dbContext) : ICommandHandler<RemoveWishlistItemCommand>
{
	public async Task<Result> Handle(RemoveWishlistItemCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.WishlistItems
			.Include(item => item.Wishlist)
			.FirstOrDefaultAsync(item => item.Id == command.Id &&
			item.Wishlist.TenantId == executionContextProvider.TenantId &&
			item.Wishlist.UserId == executionContextProvider.UserId, cancellationToken);

		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.WishlistNotFound(nameof(RemoveWishlistItemCommand.Id)));

		dbContext.WishlistItems.Remove(maybeItem);

		await dbContext.SaveChangesAsync(cancellationToken);

		return Result.Success();
	}
}
