using System.ComponentModel.DataAnnotations;
using Core.Domain.Entities;
using Core.Domain.ValueObjects;
using Document.Contract.Contracts.Enums;
using Document.Domain.Contracts.Enums;
using Document.Domain.Contracts.Events;
using Document.Domain.Shared;

namespace Document.Domain.Contracts;

public sealed class Contract : AggregateRoot<Guid>, ISoftDeleteEntity, IVersionedEntity
{
    // ─── Core ───
    public Guid TenantId { get; private set; }

    [MaxLength(50)]
    public string? ContractNumber { get; private set; }
    public Guid TemplateId { get; private set; }

    [MaxLength(5)]
    public string Language { get; private set; } = string.Empty;

    /// <summary>
    /// Resolved JSONB blocks — same format as template body, with placeholders replaced by actual values. PDF is generated on-demand from this data.
    /// </summary>
    public string Body { get; private set; } = string.Empty;

    // ─── References ───
    public Guid LeaseId { get; private set; }
    public Guid RealEstateId { get; private set; }
    public Guid OwnerCompanyId { get; private set; }
    public Guid? ClientCompanyId { get; private set; }

    // ─── Party identification ───
    [MaxLength(14)]
    public string? OwnerInn { get; private set; }

    [MaxLength(14)]
    public string? OwnerPinfl { get; private set; }

    [MaxLength(14)]
    public string? ClientInn { get; private set; }

    [MaxLength(14)]
    public string? ClientPinfl { get; private set; }

    // ─── Lease terms (denormalized) ───
    public Money MonthlyAmount { get; private set; }
    public DateOnly LeaseStartDate { get; private set; }
    public DateOnly? LeaseEndDate { get; private set; }

    // ─── Status & dates ───
    public ContractStatus Status { get; private set; } = ContractStatus.Draft;
    public DateTime ContractDate { get; private set; }
    public Guid CreatedByUserId { get; private set; }

    // ─── Versioning (IVersionedEntity) ───
    public int CurrentVersion { get; set; } = 1;

    // ─── Rejection & deadline ───
    [MaxLength(2000)]
    public string? RejectionReason { get; private set; }
    public DateTime? SignatureDeadline { get; private set; }

    // ─── Signing timestamps ───
    public DateTime? ExportedAt { get; private set; }
    public DateTime? OwnerSignedAt { get; private set; }
    public DateTime? ClientSignedAt { get; private set; }

    // ─── Hierarchy ───
    public Guid? ParentId { get; private set; }
    public Contract? Parent { get; private set; }
    public ICollection<Contract> ChildContracts { get; private set; } = [];

    // ─── Financial items ───
    private readonly List<ContractFinancialItem> _financialItems = [];
    public IReadOnlyCollection<ContractFinancialItem> FinancialItems => _financialItems.AsReadOnly();

    // ─── Integration states ───
    private readonly List<ContractProviderState> _integrationStates = [];
    public IReadOnlyCollection<ContractProviderState> IntegrationStates => _integrationStates;

    // ─── Signing events (audit) ───
    private readonly List<ContractSigningEvent> _signingEvents = [];
    public IReadOnlyCollection<ContractSigningEvent> SigningEvents => _signingEvents.AsReadOnly();

    // ─── Attachments ───
    private readonly List<ContractAttachment> _attachments = [];
    public IReadOnlyCollection<ContractAttachment> Attachments => _attachments.AsReadOnly();

    // ─── Soft delete ───
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }

    private Contract() { } // EF Core

    // ─── Factory ───

    public static Contract Create(
        Guid tenantId,
        Guid templateId,
        string language,
        string body,
        Guid leaseId,
        Guid realEstateId,
        Guid ownerCompanyId,
        Guid? clientCompanyId,
        string? ownerInn,
        string? ownerPinfl,
        string? clientInn,
        string? clientPinfl,
        Money monthlyAmount,
        DateOnly leaseStartDate,
        DateOnly? leaseEndDate,
        Guid createdByUserId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(language);
        ArgumentException.ThrowIfNullOrWhiteSpace(body);

        return new Contract
        {
            Id = Guid.CreateVersion7(),
            TenantId = tenantId,
            TemplateId = templateId,
            Language = language,
            Body = body,
            LeaseId = leaseId,
            RealEstateId = realEstateId,
            OwnerCompanyId = ownerCompanyId,
            ClientCompanyId = clientCompanyId,
            OwnerInn = ownerInn,
            OwnerPinfl = ownerPinfl,
            ClientInn = clientInn,
            ClientPinfl = clientPinfl,
            MonthlyAmount = monthlyAmount,
            LeaseStartDate = leaseStartDate,
            LeaseEndDate = leaseEndDate,
            ContractDate = DateTime.UtcNow,
            CreatedByUserId = createdByUserId,
            Status = ContractStatus.Draft
        };
    }

    /// <summary>Raises the ContractCreated domain event after construction (call after Create).</summary>
    public void RaiseCreatedEvent()
    {
        Raise(new ContractCreatedDomainEvent(Id, TenantId, TemplateId, LeaseId));
    }

    // ─── Body editing (Draft only) ───
    public void UpdateBody(string body)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(body);
        Body = body;
        Raise(new ContractBodyUpdatedDomainEvent(Id));
    }

    // ─── Regeneration (Draft only) ───
    public void Regenerate(string body)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(body);
        Body = body;
        CurrentVersion++;
        Raise(new ContractRegeneratedDomainEvent(Id, CurrentVersion));
    }

    // ─── Contract number ───
    public void SetContractNumber(string contractNumber)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(contractNumber);
        ContractNumber = contractNumber;
    }

    // ─── Status transitions ───
    public void UpdateStatus(ContractStatus status)
    {
        var oldStatus = Status;
        Status = status;
        Raise(new ContractStatusChangedDomainEvent(Id, oldStatus, status));
    }

    public void ExportToDidox(int signatureDeadlineDays = 30)
    {
        Status = ContractStatus.Sent;
        ExportedAt = DateTime.UtcNow;
        SignatureDeadline = DateTime.UtcNow.AddDays(signatureDeadlineDays);
        Raise(new ContractExportedToDidoxDomainEvent(Id));
    }

    public void MarkExpired()
    {
        Status = ContractStatus.ExpiredUnsigned;
        Raise(new ContractExpiredDomainEvent(Id));
    }

    // ─── Signing ───
    public void RecordOwnerSigned(DateTime signedAt, string? externalSignatureId = null)
    {
        OwnerSignedAt = signedAt;
        Status = ContractStatus.OwnerSigned;
        _signingEvents.Add(new ContractSigningEvent(
            Id, SigningParty.Owner, SigningAction.Signed, signedAt, externalSignatureId));
        Raise(new ContractOwnerSignedDomainEvent(Id, signedAt));
    }

    public void RecordClientSigned(DateTime signedAt, string? externalSignatureId = null)
    {
        ClientSignedAt = signedAt;
        Status = ContractStatus.FullySigned;
        _signingEvents.Add(new ContractSigningEvent(
            Id, SigningParty.Client, SigningAction.Signed, signedAt, externalSignatureId));
        Raise(new ContractClientSignedDomainEvent(Id, signedAt));
    }

    public void RecordRejected(SigningParty party, DateTime rejectedAt, string? reason = null, string? externalSignatureId = null)
    {
        Status = party == SigningParty.Owner
            ? ContractStatus.RejectedByOwner
            : ContractStatus.RejectedByClient;

        RejectionReason = reason;

        _signingEvents.Add(new ContractSigningEvent(
            Id, party, SigningAction.Rejected, rejectedAt, externalSignatureId));
        Raise(new ContractRejectedDomainEvent(Id, party, reason));
    }

    // ─── Financial items ───
    public void AddFinancialItem(
        FinancialItemType type,
        string name,
        Money amount,
        FinancialFrequency frequency,
        int sortOrder = 0)
    {
        _financialItems.Add(new ContractFinancialItem(Id, type, name, amount, frequency, sortOrder));
    }

    public void RemoveFinancialItem(Guid itemId)
    {
        var item = _financialItems.Find(x => x.Id == itemId)
            ?? throw new InvalidOperationException($"Financial item {itemId} not found");

        _financialItems.Remove(item);
    }

    // ─── Integration states ───
    public void AddOrUpdateProviderState(
        string providerName,
        string? externalId,
        ExternalSyncStatus status,
        string? errorMessage = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(providerName);

        var index = _integrationStates.FindIndex(x => x.ProviderName == providerName);
        var newState = new ContractProviderState(providerName, externalId, status, DateTime.UtcNow, errorMessage);

        if (index >= 0)
            _integrationStates[index] = newState;
        else
            _integrationStates.Add(newState);
    }

    // ─── Attachments ───

    public void AddAttachment(string fileName, string objectKey, string contentType, long fileSize, AttachmentDocumentType attachmentDocumentType, Guid uploadedByUserId)
    {
        _attachments.Add(new ContractAttachment(Id, fileName, objectKey, contentType, fileSize, attachmentDocumentType, uploadedByUserId));
    }

    public void RemoveAttachment(Guid attachmentId)
    {
        var attachment = _attachments.Find(x => x.Id == attachmentId)
            ?? throw new InvalidOperationException($"Attachment {attachmentId} not found");

        _attachments.Remove(attachment);
    }

    // ─── Parent / child ───

    public void SetParent(Guid parentId)
    {
        if (parentId == Guid.Empty)
            throw new ArgumentException("Parent ID cannot be empty", nameof(parentId));
        ParentId = parentId;
    }

    public void RemoveParent() => ParentId = null;
}
