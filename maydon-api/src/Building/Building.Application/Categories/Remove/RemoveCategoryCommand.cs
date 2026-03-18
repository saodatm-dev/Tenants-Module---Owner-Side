using Core.Application.Abstractions.Messaging;

namespace Building.Application.Categories.Remove;

public sealed record RemoveCategoryCommand(Guid Id) : ICommand;
