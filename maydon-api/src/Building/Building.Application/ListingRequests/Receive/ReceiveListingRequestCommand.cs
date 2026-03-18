using Core.Application.Abstractions.Messaging;

namespace Building.Application.ListingRequests.Receive;

public sealed record ReceiveListingRequestCommand(Guid Id) : ICommand<Guid>;
