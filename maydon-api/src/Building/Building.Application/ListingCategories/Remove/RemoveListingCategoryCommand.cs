using Core.Application.Abstractions.Messaging;

namespace Building.Application.ListingCategories.Remove;

public sealed record RemoveListingCategoryCommand(Guid Id) : ICommand;
