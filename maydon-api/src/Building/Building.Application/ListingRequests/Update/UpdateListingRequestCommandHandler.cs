using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.ListingRequests.Update;

internal sealed class UpdateListingRequestCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IExecutionContextProvider executionContextProvider,
	IBuildingDbContext dbContext) : ICommandHandler<UpdateListingRequestCommand, Guid>
{
	public async Task<Result<Guid>> Handle(UpdateListingRequestCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.ListingRequests.FirstOrDefaultAsync(item => item.Id == command.Id, cancellationToken);

		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.ListingRequestNotFound(nameof(UpdateListingRequestCommand.Id)));

		if (maybeItem.ClientId == executionContextProvider.TenantId)
			return Result.Failure<Guid>(sharedViewLocalizer.ListingRequestNoPermission(nameof(UpdateListingRequestCommand.Id)));

		dbContext.ListingRequests.Update(maybeItem.Update(command.Content));

		await dbContext.SaveChangesAsync(cancellationToken);

		return maybeItem.Id;
	}
}
