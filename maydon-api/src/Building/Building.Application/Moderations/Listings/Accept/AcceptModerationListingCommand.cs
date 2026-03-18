using Core.Application.Abstractions.Messaging;

namespace Building.Application.Moderations.Listings.Accept;

public sealed record AcceptModerationListingCommand(Guid Id) : ICommand<Guid>;
