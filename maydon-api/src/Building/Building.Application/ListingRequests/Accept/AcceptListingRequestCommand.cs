using Core.Application.Abstractions.Messaging;

namespace Building.Application.ListingRequests.Accept;

public sealed record AcceptListingRequestCommand(Guid Id) : ICommand<Guid>;
