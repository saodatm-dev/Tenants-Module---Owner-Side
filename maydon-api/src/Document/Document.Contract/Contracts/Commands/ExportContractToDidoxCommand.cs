using Core.Application.Abstractions.Messaging;

namespace Document.Contract.Contracts.Commands;

/// <summary>
/// Exports a contract to Didox for signing.
/// Transitions status to PendingSignature and locks further body edits.
/// </summary>
public sealed record ExportContractToDidoxCommand(Guid ContractId) : ICommand;
