using Core.Application.Abstractions.Messaging;

namespace Document.Contract.Contracts.Commands;

/// <summary>
/// Recalls a contract from Didox, voiding the external envelope.
/// Reverts the contract back to Draft status for re-editing.
/// </summary>
public sealed record RecallContractFromDidoxCommand(Guid ContractId) : ICommand;
