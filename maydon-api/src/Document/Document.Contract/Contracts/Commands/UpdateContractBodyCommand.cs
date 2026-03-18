using Core.Application.Abstractions.Messaging;

namespace Document.Contract.Contracts.Commands;

/// <summary>
/// Updates the JSONB body of a contract.
/// Only allowed when the contract is in Draft status.
/// </summary>
public sealed record UpdateContractBodyCommand(Guid ContractId, string Body) : ICommand;
