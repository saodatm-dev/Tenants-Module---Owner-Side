using Core.Application.Abstractions.Messaging;

namespace Document.Contract.Contracts.Commands;

/// <summary>
/// Regenerates a contract body. Only allowed in Draft status.
/// Increments the version and publishes a snapshot to the versioning system.
/// </summary>
public sealed record RegenerateContractCommand(Guid ContractId, string Body) : ICommand;
