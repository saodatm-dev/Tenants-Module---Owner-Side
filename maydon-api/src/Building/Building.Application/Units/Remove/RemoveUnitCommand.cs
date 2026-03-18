using Core.Application.Abstractions.Messaging;

namespace Building.Application.Units.Remove;

public sealed record RemoveUnitCommand(Guid Id) : ICommand;
