using Core.Application.Abstractions.Messaging;
using Core.Domain.Results;

namespace Didox.Application.Contracts.DidoxAccounts.Commands;

public record DeleteDidoxAccountCommand(Guid Id) : ICommand;

