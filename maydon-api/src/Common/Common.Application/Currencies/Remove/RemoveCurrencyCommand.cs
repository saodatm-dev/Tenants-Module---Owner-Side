using Core.Application.Abstractions.Messaging;

namespace Common.Application.Currencies.Remove;

public sealed record RemoveCurrencyCommand(Guid Id) : ICommand;
