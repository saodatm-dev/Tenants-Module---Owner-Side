using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.WishlistItems.Create;

internal sealed class CreateWishlistItemCommandHandler(
	IBuildingDbContext dbContext) : ICommandHandler<CreateWishlistItemCommand, Guid>
{
	public async Task<Result<Guid>> Handle(CreateWishlistItemCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.WishlistItems
			.IgnoreQueryFilters([IApplicationDbContext.IsDeletedFilter])
			.FirstOrDefaultAsync(item => item.WishlistId == command.WishlistId, cancellationToken);

		if(maybeItem is not null && !maybeItem.isDeleted) 
			return 


		await dbContext.WishlistItems.AddAsync(item, cancellationToken);

		await dbContext.SaveChangesAsync(cancellationToken);

		return item.Id;
	}
}
