using Core.Application.Abstractions.Messaging;

namespace Building.Application.Complexes.Remove;

public sealed record RemoveComplexCommand(Guid Id) : ICommand;
