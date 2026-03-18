using Core.Application.Abstractions.Messaging;

namespace Building.Application.Moderations.Listings.Reject;

public sealed record RejectModerationListingCommand(Guid Id, string? Reason) : ICommand<Guid>;
