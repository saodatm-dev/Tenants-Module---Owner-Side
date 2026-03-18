using Building.Application.Core.Abstractions.Data;
using Building.Domain.Wishlists.Events;
using Core.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Wishlists.Remove;

internal sealed class RemoveWishlistDomainEventHandler(IBuildingDbContext dbContext) : IDomainEventHandler<RemoveWishlistDomainEvent>
{
	public async ValueTask Handle(RemoveWishlistDomainEvent @event, CancellationToken cancellationToken)
	{
		var wishListItems = await dbContext.WishlistItems
			.Where(item => item.WishlistId == @event.Id)
			.ToListAsync(cancellationToken);

		if (wishListItems?.Count > 0)
			dbContext.WishlistItems.RemoveRange(wishListItems);

		await ValueTask.CompletedTask;
	}
}
