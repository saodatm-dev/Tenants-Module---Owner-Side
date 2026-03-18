using Core.Application.Abstractions.Messaging;

namespace Building.Application.WishlistItems.Create;

public sealed record CreateWishlistItemCommand(
	Guid WishlistId,
	Guid RealEstateId) : ICommand<Guid>;
