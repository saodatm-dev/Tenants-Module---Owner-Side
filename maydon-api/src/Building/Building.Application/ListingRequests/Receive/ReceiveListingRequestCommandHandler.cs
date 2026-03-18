using Building.Application.Core.Abstractions.Data;
using Building.Application.ListingRequests.Accept;
using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.ListingRequests.Receive;

internal sealed class ReceiveListingRequestCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IExecutionContextProvider executionContextProvider,
	IBuildingDbContext dbContext) : ICommandHandler<ReceiveListingRequestCommand, Guid>
{
	public async Task<Result<Guid>> Handle(ReceiveListingRequestCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.ListingRequests.FirstOrDefaultAsync(item => item.Id == command.Id, cancellationToken);

		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.ListingRequestNotFound(nameof(ReceiveListingRequestCommand.Id)));

		if (maybeItem.OwnerId != executionContextProvider.TenantId)
			return Result.Failure<Guid>(sharedViewLocalizer.ListingRequestNoPermission(nameof(ReceiveListingRequestCommand.Id)));

		if (maybeItem.IsReceive() || maybeItem.IsAccept())
			return maybeItem.Id;

		// check status 
		if (maybeItem.IsCancel())
			return Result.Failure<Guid>(sharedViewLocalizer.ListingRequestAlreadyCancelled(nameof(AcceptListingRequestCommand.Id)));

		if (maybeItem.IsReject())
			return Result.Failure<Guid>(sharedViewLocalizer.ListingRequestAlreadyRejected(nameof(AcceptListingRequestCommand.Id)));

		dbContext.ListingRequests.Update(maybeItem.Received());

		await dbContext.SaveChangesAsync(cancellationToken);

		return maybeItem.Id;
	}
}
