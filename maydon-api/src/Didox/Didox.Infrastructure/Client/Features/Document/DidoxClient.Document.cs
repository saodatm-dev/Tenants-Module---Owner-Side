using Didox.Application.Contracts.DidoxClient.Contracts.CommonModules;
using RestSharp;

// ReSharper disable CheckNamespace

namespace Didox.Infrastructure.Client.Features;

/// <summary>
/// Didox Client - Document Features
/// </summary>
public partial class DidoxClient
{
    /// <inheritdoc/>
    public Task<DidoxApiResponse<string>> GetHtmlDocumentAsync(string documentId, CancellationToken cancellationToken = default)
    {
        var url = _getHtmlDocumentUrl.Replace("{documentId}", documentId);
        var request = CreateRequest(url);
        
        return RestClient.ExecuteAsync(request, cancellationToken)
            .ContinueWith(t => ProcessResponse<string>(t.Result), cancellationToken);
    }
    
    /// <inheritdoc/>
    public Task<DidoxApiResponse<byte[]>> GetPdfDocumentAsync(string documentId, CancellationToken cancellationToken = default)
    {
        var request = CreateRequest(_getPdfDocumentUrl.Replace("{documentId}", documentId));
        
        return RestClient.ExecuteAsync(request, cancellationToken)
            .ContinueWith(t => ProcessResponse<byte[]>(t.Result), cancellationToken);
    }

    /// <inheritdoc/>
    public Task<DidoxApiResponse<System.Text.Json.JsonDocument>> GetDidoxJsonDoc(string documentId, string userToken, CancellationToken cancellationToken = default)
    {
        var request = CreateRequest(_getDocumentUrl.Replace("{documentId}", documentId))
            .AddHeader("user-key", userToken.Trim());
        
        return RestClient.ExecuteAsync(request, cancellationToken)
            .ContinueWith(t => ProcessResponse<System.Text.Json.JsonDocument>(t.Result), cancellationToken);
    }
}
