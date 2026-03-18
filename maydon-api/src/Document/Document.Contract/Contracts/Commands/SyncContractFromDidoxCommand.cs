using Core.Application.Abstractions.Messaging;

namespace Document.Contract.Contracts.Commands;

/// <summary>
/// Admin command to synchronize a contract's status from Didox.
/// Polls the Didox API for current envelope status and reconciles with local state.
/// </summary>
public sealed record SyncContractFromDidoxCommand(Guid ContractId) : ICommand;
