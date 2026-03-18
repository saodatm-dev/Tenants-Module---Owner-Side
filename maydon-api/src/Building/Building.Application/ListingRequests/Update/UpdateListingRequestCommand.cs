using Core.Application.Abstractions.Messaging;

namespace Building.Application.ListingRequests.Update;

public sealed record UpdateListingRequestCommand(
	Guid Id,
	string Content) : ICommand<Guid>;
