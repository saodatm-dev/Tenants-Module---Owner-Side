using Core.Application.Abstractions.Messaging;

namespace Building.Application.ListingRequests.Create;

public sealed record CreateListingRequestCommand(
	Guid ListingId,
	string Content) : ICommand<Guid>;
