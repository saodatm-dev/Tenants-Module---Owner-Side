using Core.Application.Abstractions.Messaging;

namespace Building.Application.MeterTariffs.Remove;

public sealed record RemoveMeterTariffCommand(Guid Id) : ICommand<Guid>;
