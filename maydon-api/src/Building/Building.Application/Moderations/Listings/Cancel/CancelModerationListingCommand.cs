using Core.Application.Abstractions.Messaging;

namespace Building.Application.Moderations.Listings.Cancel;

public sealed record CancelModerationListingCommand(Guid Id) : ICommand<Guid>;
