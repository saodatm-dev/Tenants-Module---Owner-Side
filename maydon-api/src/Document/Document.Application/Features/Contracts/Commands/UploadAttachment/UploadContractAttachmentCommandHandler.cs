using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Abstractions.Services.Minio;
using Core.Application.Resources;
using Core.Domain.Results;
using Document.Application.Abstractions.Data;
using Document.Contract.Contracts.Commands;
using Document.Domain.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Document.Application.Features.Contracts.Commands.UploadAttachment;

/// <summary>
/// Handles <see cref="UploadContractAttachmentCommand"/>.
/// Validates file constraints, uploads to MinIO via IFileManager, and creates the attachment record.
/// </summary>
public sealed class UploadContractAttachmentCommandHandler(
    IFileManager fileManager,
    IDocumentDbContext dbContext,
    IExecutionContextProvider executionContext,
    ISharedViewLocalizer sharedViewLocalizer,
    ILogger<UploadContractAttachmentCommandHandler> logger) : ICommandHandler<UploadContractAttachmentCommand>
{
    private const string BucketName = "contract-attachments";
    private const long MaxFileSizeBytes = 10 * 1024 * 1024;
    private const int MaxAttachmentsPerContract = 20;

    private static readonly HashSet<string> AllowedContentTypes =
    [
        "application/pdf",
        "image/png",
        "image/jpeg",
        "image/jpg",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document" // .docx
    ];

    public async Task<Result> Handle(
        UploadContractAttachmentCommand command,
        CancellationToken cancellationToken)
    {
        // Validate file size
        if (command.FileSize > MaxFileSizeBytes)
            return Result.Failure(
                sharedViewLocalizer.AttachmentTooLarge(nameof(command.FileSize), MaxFileSizeBytes / (1024 * 1024)));

        // Validate content type
        if (!AllowedContentTypes.Contains(command.ContentType.ToLowerInvariant()))
            return Result.Failure(
                sharedViewLocalizer.AttachmentInvalidType(nameof(command.ContentType)));

        // Validate contract exists
        var contract = await dbContext.Contracts
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == command.ContractId, cancellationToken);

        if (contract is null)
            return Result.Failure(
                sharedViewLocalizer.ContractNotFound(nameof(command.ContractId)));

        // Validate attachment count limit
        var existingCount = await dbContext.ContractAttachments
            .CountAsync(a => a.ContractId == command.ContractId, cancellationToken);

        if (existingCount >= MaxAttachmentsPerContract)
            return Result.Failure(
                sharedViewLocalizer.AttachmentLimitExceeded(nameof(command.ContractId), MaxAttachmentsPerContract));

        // Upload file to MinIO via IFileManager
        var uploadResult = await fileManager.UploadPrivateFileAsync(
            new FileManagerRequest(
                BucketName,
                command.FileName,
                command.ContentType,
                command.FileSize,
                command.FileStream),
            cancellationToken);

        if (uploadResult.IsFailure)
            return Result.Failure(
                sharedViewLocalizer.AttachmentUploadFailed(nameof(command.FileName)));

        var userId = executionContext.IsAuthorized ? executionContext.UserId : Guid.Empty;

        var attachment = new ContractAttachment(
            contractId: command.ContractId,
            fileName: command.FileName,
            objectKey: uploadResult.Value,
            contentType: command.ContentType,
            fileSize: command.FileSize,
            documentType: command.DocumentType,
            uploadedByUserId: userId);

        dbContext.ContractAttachments.Add(attachment);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Uploaded attachment {AttachmentId} ({FileName}) for contract {ContractId}",
            attachment.Id, command.FileName, command.ContractId);

        return Result.Success();
    }
}
