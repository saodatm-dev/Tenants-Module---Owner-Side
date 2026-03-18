using Core.Application.Abstractions.Messaging;

namespace Building.Application.Wishlists.Create;

public sealed record CreateWishlistCommand(Guid ListingId) : ICommand<Guid>;
