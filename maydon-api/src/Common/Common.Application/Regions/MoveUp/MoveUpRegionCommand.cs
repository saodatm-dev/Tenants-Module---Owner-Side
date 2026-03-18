using Core.Application.Abstractions.Messaging;

namespace Common.Application.Regions.MoveUp;

public sealed record MoveUpRegionCommand(Guid Id) : ICommand;
