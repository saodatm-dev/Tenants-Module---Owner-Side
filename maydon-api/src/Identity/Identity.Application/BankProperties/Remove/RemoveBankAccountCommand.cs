using Core.Application.Abstractions.Messaging;

namespace Identity.Application.BankProperties.Remove;

public sealed record RemoveBankAccountCommand(Guid Id) : ICommand;
