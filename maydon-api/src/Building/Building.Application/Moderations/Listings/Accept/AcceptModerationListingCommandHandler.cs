using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Moderations.Listings.Accept;

internal sealed class AcceptModerationListingCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IBuildingDbContext dbContext) : ICommandHandler<AcceptModerationListingCommand, Guid>
{
	public async Task<Result<Guid>> Handle(AcceptModerationListingCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.Listings
			.IgnoreQueryFilters([IApplicationDbContext.TenantIdFilter])
			.FirstOrDefaultAsync(item => item.Id == command.Id, cancellationToken);

		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.ListingNotFound(nameof(AcceptModerationListingCommand.Id)));

		if (maybeItem.IsAccept())
			return maybeItem.Id;

		// check status 
		if (maybeItem.IsCancel())
			return Result.Failure<Guid>(sharedViewLocalizer.WasCancelledByUser(nameof(AcceptModerationListingCommand.Id)));

		dbContext.Listings.Update(maybeItem.Accept());

		await dbContext.SaveChangesAsync(cancellationToken);

		return maybeItem.Id;
	}
}
