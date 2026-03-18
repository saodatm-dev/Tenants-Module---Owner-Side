using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Moderations.Listings.Reject;

internal sealed class RejectModerationListingCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IBuildingDbContext dbContext) : ICommandHandler<RejectModerationListingCommand, Guid>
{
	public async Task<Result<Guid>> Handle(RejectModerationListingCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.Listings
			.IgnoreQueryFilters([IApplicationDbContext.TenantIdFilter])
			.FirstOrDefaultAsync(item => item.Id == command.Id, cancellationToken);

		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.ListingNotFound(nameof(RejectModerationListingCommand.Id)));

		if (maybeItem.IsReject())
			return maybeItem.Id;

		// check status 
		if (maybeItem.IsCancel())
			return Result.Failure<Guid>(sharedViewLocalizer.WasCancelledByUser(nameof(RejectModerationListingCommand.Id)));

		dbContext.Listings.Update(maybeItem.Reject(command.Reason));

		await dbContext.SaveChangesAsync(cancellationToken);

		return maybeItem.Id;
	}
}
