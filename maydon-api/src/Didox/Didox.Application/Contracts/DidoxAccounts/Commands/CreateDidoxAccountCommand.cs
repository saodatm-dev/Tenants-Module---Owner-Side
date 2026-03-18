using Core.Application.Abstractions.Messaging;
using Core.Domain.Results;

using Didox.Application.Contracts.DidoxAccounts.Responses;

namespace Didox.Application.Contracts.DidoxAccounts.Commands;

public record CreateDidoxAccountCommand(
    string Login,
    string Password
) : ICommand<DidoxAccountResponse>;

