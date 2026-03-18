using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.ListingRequests.Cancel;

internal sealed class CancelListingRequestCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IExecutionContextProvider executionContextProvider,
	IBuildingDbContext dbContext) : ICommandHandler<CancelListingRequestCommand, Guid>
{
	public async Task<Result<Guid>> Handle(CancelListingRequestCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.ListingRequests.FirstOrDefaultAsync(item => item.Id == command.Id, cancellationToken);

		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.ListingRequestNotFound(nameof(CancelListingRequestCommand.Id)));

		if (maybeItem.ClientId != executionContextProvider.TenantId)
			return Result.Failure<Guid>(sharedViewLocalizer.ListingRequestNoPermission(nameof(CancelListingRequestCommand.Id)));

		if (maybeItem.IsCancel())
			return maybeItem.Id;

		// check status 
		if (maybeItem.IsAccept())
			return Result.Failure<Guid>(sharedViewLocalizer.ListingRequestWasAcceptedByOwner(nameof(CancelListingRequestCommand.Id)));

		if (maybeItem.IsReject())
			return Result.Failure<Guid>(sharedViewLocalizer.ListingRequestAlreadyRejected(nameof(CancelListingRequestCommand.Id)));

		dbContext.ListingRequests.Update(maybeItem.Cancel());

		await dbContext.SaveChangesAsync(cancellationToken);

		return maybeItem.Id;
	}
}
