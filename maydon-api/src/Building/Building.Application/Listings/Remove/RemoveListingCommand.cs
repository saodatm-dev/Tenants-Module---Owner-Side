using Core.Application.Abstractions.Messaging;

namespace Building.Application.Listings.Remove;

public sealed record RemoveListingCommand(Guid Id) : ICommand;
