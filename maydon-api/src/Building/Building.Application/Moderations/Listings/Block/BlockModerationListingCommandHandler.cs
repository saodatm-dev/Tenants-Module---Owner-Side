using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Moderations.Listings.Block;

internal sealed class BlockModerationListingCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IBuildingDbContext dbContext) : ICommandHandler<BlockModerationListingCommand, Guid>
{
	public async Task<Result<Guid>> Handle(BlockModerationListingCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.Listings
			.IgnoreQueryFilters([IApplicationDbContext.TenantIdFilter])
			.FirstOrDefaultAsync(item => item.Id == command.Id, cancellationToken);

		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.ListingNotFound(nameof(BlockModerationListingCommand.Id)));

		if (maybeItem.IsBlocked())
			return maybeItem.Id;

		// check status 
		if (maybeItem.IsCancel())
			return Result.Failure<Guid>(sharedViewLocalizer.WasCancelledByUser(nameof(BlockModerationListingCommand.Id)));

		dbContext.Listings.Update(maybeItem.Block(command.Reason));

		await dbContext.SaveChangesAsync(cancellationToken);

		return maybeItem.Id;
	}
}
