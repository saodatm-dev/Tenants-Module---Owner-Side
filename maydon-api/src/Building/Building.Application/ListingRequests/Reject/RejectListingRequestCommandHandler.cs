using Building.Application.Core.Abstractions.Data;
using Building.Application.ListingRequests.Accept;
using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.ListingRequests.Reject;

internal sealed class RejectListingRequestCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IExecutionContextProvider executionContextProvider,
	IBuildingDbContext dbContext) : ICommandHandler<RejectListingRequestCommand, Guid>
{
	public async Task<Result<Guid>> Handle(RejectListingRequestCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.ListingRequests.FirstOrDefaultAsync(item => item.Id == command.Id, cancellationToken);

		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.ListingRequestNotFound(nameof(RejectListingRequestCommand.Id)));

		if (maybeItem.OwnerId != executionContextProvider.TenantId)
			return Result.Failure<Guid>(sharedViewLocalizer.ListingRequestNoPermission(nameof(RejectListingRequestCommand.Id)));

		if (maybeItem.IsReject())
			return maybeItem.Id;

		// check status 
		if (maybeItem.IsCancel())
			return Result.Failure<Guid>(sharedViewLocalizer.ListingRequestAlreadyCancelled(nameof(AcceptListingRequestCommand.Id)));

		dbContext.ListingRequests.Update(maybeItem.Reject(command.Reason));

		await dbContext.SaveChangesAsync(cancellationToken);

		return maybeItem.Id;
	}
}
