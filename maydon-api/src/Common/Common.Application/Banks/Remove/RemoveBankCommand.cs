using Core.Application.Abstractions.Messaging;

namespace Common.Application.Banks.Remove;

public sealed record RemoveBankCommand(Guid Id) : ICommand;
