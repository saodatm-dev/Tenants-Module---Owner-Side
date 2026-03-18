using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.ListingRequests.Remove;

internal sealed class RemoveListingRequestCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IExecutionContextProvider executionContextProvider,
	IBuildingDbContext dbContext) : ICommandHandler<RemoveListingRequestCommand, Guid>
{
	public async Task<Result<Guid>> Handle(RemoveListingRequestCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.ListingRequests.FirstOrDefaultAsync(item => item.Id == command.Id, cancellationToken);

		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.ListingRequestNotFound(nameof(RemoveListingRequestCommand.Id)));

		if (maybeItem.ClientId == executionContextProvider.TenantId)
			return Result.Failure<Guid>(sharedViewLocalizer.ListingRequestNoPermission(nameof(RemoveListingRequestCommand.Id)));

		if (maybeItem.IsAccept())
			return Result.Failure<Guid>(sharedViewLocalizer.ListingRequestWasAcceptedByOwner(nameof(RemoveListingRequestCommand.Id)));

		dbContext.ListingRequests.Remove(maybeItem);

		await dbContext.SaveChangesAsync(cancellationToken);

		return maybeItem.Id;
	}
}
