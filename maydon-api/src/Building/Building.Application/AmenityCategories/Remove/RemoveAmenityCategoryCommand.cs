using Core.Application.Abstractions.Messaging;

namespace Building.Application.AmenityCategories.Remove;

public sealed record RemoveAmenityCategoryCommand(Guid Id) : ICommand;
