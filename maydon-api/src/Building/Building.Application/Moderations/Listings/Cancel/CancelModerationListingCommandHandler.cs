using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Moderations.Listings.Cancel;

internal sealed class CancelModerationListingCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IBuildingDbContext dbContext) : ICommandHandler<CancelModerationListingCommand, Guid>
{
	public async Task<Result<Guid>> Handle(CancelModerationListingCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.Listings
			.IgnoreQueryFilters([IApplicationDbContext.TenantIdFilter])
			.FirstOrDefaultAsync(item => item.Id == command.Id, cancellationToken);

		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.ListingNotFound(nameof(CancelModerationListingCommand.Id)));

		if (maybeItem.IsCancel())
			return maybeItem.Id;

		// check status 
		if (maybeItem.IsBlocked())
			return Result.Failure<Guid>(sharedViewLocalizer.WasBlockedByModerator(nameof(CancelModerationListingCommand.Id)));

		if (maybeItem.IsReject())
			return Result.Failure<Guid>(sharedViewLocalizer.WasRejectedByModerator(nameof(CancelModerationListingCommand.Id)));

		if (maybeItem.IsAccept())
			return Result.Failure<Guid>(sharedViewLocalizer.WasAcceptedByModerator(nameof(CancelModerationListingCommand.Id)));

		dbContext.Listings.Update(maybeItem.Cancel());

		await dbContext.SaveChangesAsync(cancellationToken);

		return maybeItem.Id;
	}
}
