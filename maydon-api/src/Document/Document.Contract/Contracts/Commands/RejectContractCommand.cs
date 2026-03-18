using Core.Application.Abstractions.Messaging;

namespace Document.Contract.Contracts.Commands;

/// <summary>
/// Rejects a contract by the specified party (owner or client).
/// Transitions to RejectedByOwner or RejectedByClient depending on the party.
/// </summary>
public sealed record RejectContractCommand(
    Guid ContractId,
    string Party,
    string? Reason = null) : ICommand;
