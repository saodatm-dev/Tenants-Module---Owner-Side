using Core.Application.Abstractions.Messaging;

namespace Building.Application.ListingRequests.Send;

public sealed record SendListingRequestCommand(Guid Id) : ICommand<Guid>;
