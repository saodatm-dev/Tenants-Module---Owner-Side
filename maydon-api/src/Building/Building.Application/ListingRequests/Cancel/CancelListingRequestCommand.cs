using Core.Application.Abstractions.Messaging;

namespace Building.Application.ListingRequests.Cancel;

public sealed record CancelListingRequestCommand(Guid Id) : ICommand<Guid>;
