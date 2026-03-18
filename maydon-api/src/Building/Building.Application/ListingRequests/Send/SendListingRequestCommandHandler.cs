using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.ListingRequests.Send;

internal sealed class SendListingRequestCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IExecutionContextProvider executionContextProvider,
	IBuildingDbContext dbContext) : ICommandHandler<SendListingRequestCommand, Guid>
{
	public async Task<Result<Guid>> Handle(SendListingRequestCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.ListingRequests.FirstOrDefaultAsync(item => item.Id == command.Id, cancellationToken);

		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.ListingRequestNotFound(nameof(SendListingRequestCommand.Id)));

		if (maybeItem.ClientId != executionContextProvider.TenantId)
			return Result.Failure<Guid>(sharedViewLocalizer.ListingRequestNoPermission(nameof(SendListingRequestCommand.Id)));

		if (maybeItem.IsSent())
			return maybeItem.Id;

		// check status 
		if (maybeItem.IsAccept())
			return Result.Failure<Guid>(sharedViewLocalizer.ListingRequestWasAcceptedByOwner(nameof(SendListingRequestCommand.Id)));

		dbContext.ListingRequests.Update(maybeItem.Send());

		await dbContext.SaveChangesAsync(cancellationToken);

		return maybeItem.Id;
	}
}
