using Core.Application.Abstractions.Messaging;

namespace Building.Application.Buildings.Remove;

public sealed record RemoveBuildingCommand(Guid Id) : ICommand;
