using Core.Application.Abstractions.Messaging;

namespace Building.Application.Floors.Remove;

public sealed record RemoveFloorCommand(Guid Id) : ICommand;
