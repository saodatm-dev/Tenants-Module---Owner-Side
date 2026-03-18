using Core.Application.Abstractions.Messaging;

namespace Common.Application.Districts.Remove;

public sealed record RemoveDistrictCommand(Guid Id) : ICommand;
