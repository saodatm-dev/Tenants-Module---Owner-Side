using Core.Application.Abstractions.Messaging;

namespace Building.Application.MeterTypes.Remove;

public sealed record RemoveMeterTypeCommand(Guid Id) : ICommand<Guid>;
