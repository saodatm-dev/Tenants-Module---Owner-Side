using Core.Application.Abstractions.Messaging;

namespace Building.Application.ProductionTypes.Remove;

public sealed record RemoveProductionTypeCommand(Guid Id) : ICommand;
