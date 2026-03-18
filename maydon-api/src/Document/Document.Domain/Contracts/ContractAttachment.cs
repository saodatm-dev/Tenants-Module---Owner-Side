using System.ComponentModel.DataAnnotations;
using Core.Domain.Entities;
using Document.Contract.Contracts.Enums;
using Document.Domain.Contracts.Enums;

namespace Document.Domain.Contracts;

public sealed class ContractAttachment : EntityBase<Guid>
{
    public Guid ContractId { get; private set; }

    [MaxLength(256)]
    public string FileName { get; private set; } = string.Empty;

    [MaxLength(512)]
    public string ObjectKey { get; private set; } = string.Empty;

    [MaxLength(128)]
    public string ContentType { get; private set; } = string.Empty;

    public long FileSize { get; private set; }
    public AttachmentDocumentType DocumentType { get; private set; }
    public DateTime UploadedAt { get; private set; }
    public Guid UploadedByUserId { get; private set; }

    private ContractAttachment() { } // EF Core

    public ContractAttachment(
        Guid contractId,
        string fileName,
        string objectKey,
        string contentType,
        long fileSize,
        AttachmentDocumentType documentType,
        Guid uploadedByUserId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
        ArgumentException.ThrowIfNullOrWhiteSpace(objectKey);

        Id = Guid.CreateVersion7();
        ContractId = contractId;
        FileName = fileName;
        ObjectKey = objectKey;
        ContentType = contentType;
        FileSize = fileSize;
        DocumentType = documentType;
        UploadedAt = DateTime.UtcNow;
        UploadedByUserId = uploadedByUserId;
    }
}
