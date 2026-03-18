using Document.Contract.Contracts.Enums;

namespace Document.Contract.Contracts.Responses;

/// <summary>
/// Response DTO for contract attachment metadata.
/// </summary>
public sealed record ContractAttachmentResponse(
    Guid Id,
    Guid ContractId,
    string FileName,
    string ContentType,
    long FileSize,
    AttachmentDocumentType DocumentType,
    DateTime UploadedAt,
    Guid UploadedByUserId,
    string? DownloadUrl);
