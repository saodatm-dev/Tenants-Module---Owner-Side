using Core.Application.Abstractions.Messaging;

namespace Building.Application.ListingRequests.Reject;

public sealed record RejectListingRequestCommand(Guid Id, string Reason) : ICommand<Guid>;
