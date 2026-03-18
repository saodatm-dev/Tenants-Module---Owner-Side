using Building.Application.Core.Abstractions.Data;
using Building.Domain.ListingRequests;
using Building.Domain.Listings;
using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.ListingRequests.Create;

internal sealed class CreateListingRequestCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IExecutionContextProvider executionContextProvider,
	IBuildingDbContext dbContext) : ICommandHandler<CreateListingRequestCommand, Guid>
{
	public async Task<Result<Guid>> Handle(CreateListingRequestCommand command, CancellationToken cancellationToken)
	{
		var listing = await dbContext.Listings
			.AsNoTracking()
			.IgnoreQueryFilters([IApplicationDbContext.TenantIdFilter])
			.FirstOrDefaultAsync(item => item.Id == command.ListingId, cancellationToken);

		if (listing is null)
			return Result.Failure<Guid>(sharedViewLocalizer.ListingNotFound(nameof(CreateListingRequestCommand.ListingId)));

		if (listing.OwnerId == executionContextProvider.TenantId)
			return Result.Failure<Guid>(sharedViewLocalizer.OwnerCanNotMakeRequestToOwnListing(nameof(CreateListingRequestCommand.ListingId)));

		if (!listing.IsActive())
			return Result.Failure<Guid>(sharedViewLocalizer.InactiveListingCannotAcceptRequests(nameof(CreateListingRequestCommand.ListingId)));

		var item = new ListingRequest(
			listing.Id,
			listing.OwnerId,
			executionContextProvider.TenantId,
			command.Content);

		await dbContext.ListingRequests.AddAsync(item, cancellationToken);

		await dbContext.SaveChangesAsync(cancellationToken);

		return item.Id;
	}
}
