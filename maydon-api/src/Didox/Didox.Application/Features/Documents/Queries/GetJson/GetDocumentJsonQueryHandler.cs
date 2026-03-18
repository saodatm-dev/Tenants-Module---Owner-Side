using System.Text.Json;
using Core.Domain.Results;
using Core.Application.Resources;
using Core.Application.Abstractions.Messaging;
using Didox.Application.Services;
using Didox.Application.Abstractions.Client;
using Didox.Application.Contracts.DidoxDocuments.Queries;
using Microsoft.Extensions.Logging;

namespace Didox.Application.Features.Documents.Queries.GetJson;

public class GetDocumentJsonQueryHandler(
    ISharedViewLocalizer sharedLocalizer,
    ILogger<GetDocumentJsonQueryHandler> logger,
    IDidoxClient didoxClient,
    IDidoxAuthService didoxAuthService)
    : IQueryHandler<GetDocumentJsonQuery, JsonDocument>
{
    public async Task<Result<JsonDocument>> Handle(GetDocumentJsonQuery query, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting JSON document.DocumentId={DocumentId}",query.DocumentId);
        
        var token = await didoxAuthService.GetActiveTokenAsync(cancellationToken);
        if (token.IsFailure)
        {
            logger.LogWarning("Token not found. DocumentId={DocumentId}", query.DocumentId);
            return Result.Failure<JsonDocument>(token.Error);
        }
        
        var response = await didoxClient.GetDidoxJsonDoc(query.DocumentId, token.Value, cancellationToken);
        
        if (!response.Success)
        {
            logger.LogError("Failed to get JSON document. DocumentId={DocumentId}, StatusCode={StatusCode}, Error={Error}", query.DocumentId, response.StatusCode, response.ErrorMessage);
            return Result.Failure<JsonDocument>(sharedLocalizer.DidoxRequestFailed(response.ErrorMessage ?? response.StatusCode.ToString()));
        }
        
        logger.LogInformation("JSON document retrieved successfully. DocumentId={DocumentId}", query.DocumentId);
        return Result.Success(response.Data!);
    }
}

