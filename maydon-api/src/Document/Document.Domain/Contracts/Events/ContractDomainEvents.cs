using Core.Domain.Events;
using Document.Domain.Contracts.Enums;

namespace Document.Domain.Contracts.Events;

/// <summary>Raised when a new contract is created in Draft status.</summary>
public sealed record ContractCreatedDomainEvent(
    Guid ContractId,
    Guid TenantId,
    Guid TemplateId,
    Guid LeaseId) : IPostPublishDomainEvent;

/// <summary>Raised when the contract JSONB body is updated.</summary>
public sealed record ContractBodyUpdatedDomainEvent(
    Guid ContractId) : IPostPublishDomainEvent;

/// <summary>Raised when the contract body is regenerated (version incremented).</summary>
public sealed record ContractRegeneratedDomainEvent(
    Guid ContractId,
    int NewVersion) : IPostPublishDomainEvent;

/// <summary>Raised when the contract status changes.</summary>
public sealed record ContractStatusChangedDomainEvent(
    Guid ContractId,
    ContractStatus OldStatus,
    ContractStatus NewStatus) : IPostPublishDomainEvent;

/// <summary>Raised when the contract is exported to Didox for signing.</summary>
public sealed record ContractExportedToDidoxDomainEvent(
    Guid ContractId) : IPostPublishDomainEvent;



/// <summary>Raised when a contract expires due to signature deadline passing.</summary>
public sealed record ContractExpiredDomainEvent(
    Guid ContractId) : IPostPublishDomainEvent;

/// <summary>Raised when the owner signs the contract.</summary>
public sealed record ContractOwnerSignedDomainEvent(
    Guid ContractId,
    DateTime SignedAt) : IPostPublishDomainEvent;

/// <summary>Raised when the client signs the contract (fully signed).</summary>
public sealed record ContractClientSignedDomainEvent(
    Guid ContractId,
    DateTime SignedAt) : IPostPublishDomainEvent;

/// <summary>Raised when a contract is rejected by a party.</summary>
public sealed record ContractRejectedDomainEvent(
    Guid ContractId,
    SigningParty Party,
    string? Reason) : IPostPublishDomainEvent;
