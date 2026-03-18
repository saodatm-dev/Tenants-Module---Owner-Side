using Core.Application.Abstractions.Messaging;

namespace Building.Application.WishlistItems.Remove;

public sealed record RemoveWishlistItemCommand(Guid Id) : ICommand;
