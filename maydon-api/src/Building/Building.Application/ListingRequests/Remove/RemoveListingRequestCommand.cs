using Core.Application.Abstractions.Messaging;

namespace Building.Application.ListingRequests.Remove;

public sealed record RemoveListingRequestCommand(Guid Id) : ICommand<Guid>;
