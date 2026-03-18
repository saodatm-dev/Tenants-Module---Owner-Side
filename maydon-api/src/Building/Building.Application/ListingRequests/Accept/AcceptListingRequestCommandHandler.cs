using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.ListingRequests.Accept;

internal sealed class AcceptListingRequestCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IExecutionContextProvider executionContextProvider,
	IBuildingDbContext dbContext) : ICommandHandler<AcceptListingRequestCommand, Guid>
{
	public async Task<Result<Guid>> Handle(AcceptListingRequestCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.ListingRequests.FirstOrDefaultAsync(item => item.Id == command.Id, cancellationToken);

		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.ListingRequestNotFound(nameof(AcceptListingRequestCommand.Id)));

		if (maybeItem.OwnerId != executionContextProvider.TenantId)
			return Result.Failure<Guid>(sharedViewLocalizer.ListingRequestNoPermission(nameof(AcceptListingRequestCommand.Id)));

		if (maybeItem.IsAccept())
			return maybeItem.Id;

		// check status 
		if (maybeItem.IsCancel())
			return Result.Failure<Guid>(sharedViewLocalizer.ListingRequestAlreadyCancelled(nameof(AcceptListingRequestCommand.Id)));

		if (maybeItem.IsReject())
			return Result.Failure<Guid>(sharedViewLocalizer.ListingRequestAlreadyRejected(nameof(AcceptListingRequestCommand.Id)));

		dbContext.ListingRequests.Update(maybeItem.Accept());

		await dbContext.SaveChangesAsync(cancellationToken);

		return maybeItem.Id;
	}
}
