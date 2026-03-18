using Core.Application.Abstractions.Messaging;

namespace Common.Application.Districts.MoveDown;

public sealed record MoveDownDistrictCommand(Guid Id) : ICommand;
