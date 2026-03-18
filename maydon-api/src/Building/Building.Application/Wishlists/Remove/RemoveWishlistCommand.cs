using Core.Application.Abstractions.Messaging;

namespace Building.Application.Wishlists.Remove;

public sealed record RemoveWishlistCommand(Guid ListingId) : ICommand;
