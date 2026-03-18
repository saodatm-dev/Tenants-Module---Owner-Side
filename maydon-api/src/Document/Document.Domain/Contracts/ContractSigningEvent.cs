using System.ComponentModel.DataAnnotations;
using Core.Domain.Entities;
using Document.Domain.Contracts.Enums;

namespace Document.Domain.Contracts;

public sealed class ContractSigningEvent : EntityBase<Guid>
{
    public Guid ContractId { get; private set; }
    public SigningParty Party { get; private set; }
    public SigningAction Action { get; private set; }
    public DateTime OccurredAt { get; private set; }

    [MaxLength(256)]
    public string? ExternalSignatureId { get; private set; }

    private ContractSigningEvent() { } // EF Core

    public ContractSigningEvent(
        Guid contractId,
        SigningParty party,
        SigningAction action,
        DateTime occurredAt,
        string? externalSignatureId = null)
    {
        Id = Guid.CreateVersion7();
        ContractId = contractId;
        Party = party;
        Action = action;
        OccurredAt = occurredAt;
        ExternalSignatureId = externalSignatureId;
    }
}
