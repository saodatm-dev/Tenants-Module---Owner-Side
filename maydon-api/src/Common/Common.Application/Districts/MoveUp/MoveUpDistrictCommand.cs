using Core.Application.Abstractions.Messaging;

namespace Common.Application.Districts.MoveUp;

public sealed record MoveUpDistrictCommand(Guid Id) : ICommand;
