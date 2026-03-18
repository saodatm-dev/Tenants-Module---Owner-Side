using Core.Application.Abstractions.Messaging;

namespace Common.Application.Regions.MoveDown;

public sealed record MoveDownRegionCommand(Guid Id) : ICommand;
