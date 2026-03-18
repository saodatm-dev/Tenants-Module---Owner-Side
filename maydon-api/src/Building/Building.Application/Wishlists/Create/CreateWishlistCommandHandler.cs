using Building.Application.Core.Abstractions.Data;
using Building.Domain.Wishlists;
using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Wishlists.Create;

internal sealed class CreateWishlistCommandHandler(
	IExecutionContextProvider executionContextProvider,
	IBuildingDbContext dbContext) : ICommandHandler<CreateWishlistCommand, Guid>
{
	public async Task<Result<Guid>> Handle(CreateWishlistCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.Wishlists
			.IgnoreQueryFilters([IApplicationDbContext.IsDeletedFilter])
			.FirstOrDefaultAsync(item => item.Id == command.ListingId, cancellationToken);

		if (maybeItem is not null && !maybeItem.IsDeleted)
			return maybeItem.Id;

		if (maybeItem is not null && maybeItem.IsDeleted)
			dbContext.Wishlists.Update(maybeItem.Restore());
		else
		{
			maybeItem = new Wishlist(
				executionContextProvider.TenantId,
				executionContextProvider.UserId,
				command.ListingId);

			await dbContext.Wishlists.AddAsync(maybeItem, cancellationToken);
		}

		await dbContext.SaveChangesAsync(cancellationToken);

		return maybeItem.Id;
	}
}
