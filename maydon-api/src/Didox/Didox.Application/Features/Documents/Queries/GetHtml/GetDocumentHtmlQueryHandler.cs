using Core.Domain.Results;
using Core.Application.Resources;
using Core.Application.Abstractions.Messaging;
using Didox.Application.Abstractions.Client;
using Didox.Application.Contracts.DidoxDocuments.Queries;
using Microsoft.Extensions.Logging;

namespace Didox.Application.Features.Documents.Queries.GetHtml;

public class GetDocumentHtmlQueryHandler(
    ILogger<GetDocumentHtmlQueryHandler> logger,
    ISharedViewLocalizer sharedLocalizer,
    IDidoxClient didoxClient)
    : IQueryHandler<GetDocumentHtmlQuery, string>
{
    public async Task<Result<string>> Handle(GetDocumentHtmlQuery query, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting HTML document. DocumentId={DocumentId}", query.DocumentId);
        
        var response = await didoxClient.GetHtmlDocumentAsync(query.DocumentId, cancellationToken);
        
        if (!response.Success)
        {
            logger.LogError("Failed to get HTML document. DocumentId={DocumentId}, StatusCode={StatusCode}, Error={Error}", 
                query.DocumentId, response.StatusCode, response.ErrorMessage);
            
             return Result.Failure<string>(sharedLocalizer.DidoxRequestFailed(response.ErrorMessage ?? response.StatusCode.ToString()));
        }
        
        logger.LogInformation("HTML document retrieved successfully. DocumentId={DocumentId}", query.DocumentId);
        return Result.Success(response.Html!);
    }
}

