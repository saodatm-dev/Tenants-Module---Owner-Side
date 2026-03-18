using Core.Application.Abstractions.Messaging;

namespace Building.Application.Moderations.Listings.Block;

public sealed record BlockModerationListingCommand(Guid Id, string? Reason = null) : ICommand<Guid>;
