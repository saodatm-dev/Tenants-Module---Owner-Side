using Core.Application.Abstractions.Messaging;
using Core.Application.Abstractions.Services.Minio;
using Core.Application.Resources;
using Core.Domain.Results;
using Document.Application.Abstractions.Data;
using Document.Contract.Contracts.Queries;
using Document.Contract.Contracts.Responses;
using Microsoft.EntityFrameworkCore;

namespace Document.Application.Features.Contracts.Queries.ListAttachments;

/// <summary>
/// Handles <see cref="ListContractAttachmentsQuery"/>.
/// Returns all attachment metadata for a given contract, including presigned download URLs.
/// </summary>
public sealed class ListContractAttachmentsQueryHandler(
    IDocumentDbContext dbContext,
    IFileManager fileManager,
    ISharedViewLocalizer sharedViewLocalizer) : IQueryHandler<ListContractAttachmentsQuery, IReadOnlyList<ContractAttachmentResponse>>
{
    public async Task<Result<IReadOnlyList<ContractAttachmentResponse>>> Handle(
        ListContractAttachmentsQuery query,
        CancellationToken cancellationToken)
    {
        var contractExists = await dbContext.Contracts
            .AnyAsync(c => c.Id == query.ContractId, cancellationToken);

        if (!contractExists)
            return Result.Failure<IReadOnlyList<ContractAttachmentResponse>>(
                sharedViewLocalizer.ContractNotFound(nameof(query.ContractId)));

        var attachments = await dbContext.ContractAttachments
            .Where(a => a.ContractId == query.ContractId)
            .OrderByDescending(a => a.UploadedAt)
            .Select(a => new
            {
                a.Id,
                a.ContractId,
                a.FileName,
                a.ContentType,
                a.FileSize,
                a.DocumentType,
                a.UploadedAt,
                a.UploadedByUserId,
                a.ObjectKey
            })
            .ToListAsync(cancellationToken);

        var responses = new List<ContractAttachmentResponse>(attachments.Count);

        foreach (var a in attachments)
        {
            var urlResult = await fileManager.GetPresignedUrlAsync(a.ObjectKey, 3600, cancellationToken);

            responses.Add(new ContractAttachmentResponse(
                a.Id,
                a.ContractId,
                a.FileName,
                a.ContentType,
                a.FileSize,
                a.DocumentType,
                a.UploadedAt,
                a.UploadedByUserId,
                urlResult.IsSuccess ? urlResult.Value : null));
        }

        return responses;
    }
}
