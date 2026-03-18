using Core.Application.Abstractions.Messaging;
using Core.Domain.Results;

namespace Didox.Application.Contracts.DidoxAccounts.Commands;

public record UpdateDidoxAccountCommand(
    Guid Id,
    string? Login,
    string? Password
) : ICommand;

