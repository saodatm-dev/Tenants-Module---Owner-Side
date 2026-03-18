using System.Text;
using Core.Domain.Results;
using Core.Application.Resources;
using Core.Application.Abstractions.Messaging;
using Didox.Application.Abstractions.Client;
using Didox.Application.Contracts.DidoxDocuments.Queries;
using Microsoft.Extensions.Logging;

namespace Didox.Application.Features.Documents.Queries.GetPdf;

public class GetDocumentPdfQueryHandler(
    ILogger<GetDocumentPdfQueryHandler> logger,
    IDidoxClient didoxClient,
    ISharedViewLocalizer sharedLocalizer)
    : IQueryHandler<GetDocumentPdfQuery, byte[]>
{
    public async Task<Result<byte[]>> Handle(GetDocumentPdfQuery query, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting PDF document. DocumentId={DocumentId}", query.DocumentId);
        
        var response = await didoxClient.GetPdfDocumentAsync(query.DocumentId, cancellationToken);
        
        if (!response.Success)
        {
            logger.LogError("Failed to get PDF document. DocumentId={DocumentId}, StatusCode={StatusCode}, Error={Error}", query.DocumentId, response.StatusCode, response.ErrorMessage);
            return Result.Failure<byte[]>(sharedLocalizer.DidoxRequestFailed(response.ErrorMessage ?? response.StatusCode.ToString()));
        }
        
        if (response.Pdf == null || response.Pdf?.Length == 0)
        {
            logger.LogError("PDF document is empty. DocumentId={DocumentId}", query.DocumentId);
            return Result.Failure<byte[]>(sharedLocalizer.InvalidValue("PdfDocumentIsEmpty"));
        }
        
        var pdfSignature = Encoding.UTF8.GetString(response.Pdf?.Take(5).ToArray() ?? Array.Empty<byte>());
        if (!pdfSignature.StartsWith("%PDF-"))
        {
            logger.LogError("Invalid PDF signature. DocumentId={DocumentId}, Signature={Signature}", query.DocumentId, pdfSignature);
            return Result.Failure<byte[]>(sharedLocalizer.InvalidValue("InvalidPdfFormat"));
        }
        
        logger.LogInformation("PDF document retrieved successfully.DocumentId={DocumentId}, Size={Size} bytes", 
            query.DocumentId, response.Pdf?.Length);
        return Result.Success(response.Pdf);
    }
}
