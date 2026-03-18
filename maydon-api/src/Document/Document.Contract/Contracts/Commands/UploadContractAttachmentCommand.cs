using Core.Application.Abstractions.Messaging;
using Document.Contract.Contracts.Enums;

namespace Document.Contract.Contracts.Commands;

/// <summary>
/// Uploads an attachment to a contract.
/// The file stream is provided by the API layer from the multipart form upload.
/// </summary>
public sealed record UploadContractAttachmentCommand(
    Guid ContractId,
    string FileName,
    string ContentType,
    long FileSize,
    AttachmentDocumentType DocumentType,
    Stream FileStream) : ICommand;
