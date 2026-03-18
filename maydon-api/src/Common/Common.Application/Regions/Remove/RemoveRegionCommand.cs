using Core.Application.Abstractions.Messaging;

namespace Common.Application.Regions.Remove;

public sealed record RemoveRegionCommand(Guid Id) : ICommand;
