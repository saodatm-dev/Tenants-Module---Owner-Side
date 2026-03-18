using Core.Application.Abstractions.Messaging;

namespace Building.Application.Meters.Remove;

public sealed record RemoveMeterCommand(Guid Id) : ICommand;
