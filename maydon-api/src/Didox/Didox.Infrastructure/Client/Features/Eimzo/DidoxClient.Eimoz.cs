using Didox.Application.Contracts.DidoxClient.Contracts.CommonModules;
using RestSharp;
// ReSharper disable CheckNamespace

namespace Didox.Infrastructure.Client.Features;

/// <summary>
/// Partial class for EIMZO/signature endpoints
/// </summary>
public partial class DidoxClient
{
    /// <inheritdoc/>
    public async Task<DidoxApiResponse<bool>> SignDocumentAsync(string signature, string documentId, string userToken, CancellationToken cancellationToken = default)
    {
        var request = CreateRequest(_documentSignUrl.Replace("{documentId}", documentId), Method.Post)
            .AddHeader("user-key", userToken.Trim())
            .AddJsonBody(new
            {
                signature
            });
        
        return await RestClient.ExecuteAsync(request, cancellationToken)
            .ContinueWith(t => ProcessResponse<bool>(t.Result), cancellationToken);
    }
    
    /// <inheritdoc/>
    public async Task<DidoxApiResponse<bool>> RejectDocumentAsync(string signature, string comment, string documentId, string tokenValue, CancellationToken cancellationToken)
    {
        var request = CreateRequest(_documentRejectUrl.Replace("{documentId}", documentId), Method.Post)
            .AddHeader("user-key", tokenValue.Trim())
            .AddJsonBody(new
            {
                signature,
                comment
            });
        
        return await RestClient.ExecuteAsync(request, cancellationToken)
            .ContinueWith(t => ProcessResponse<bool>(t.Result), cancellationToken);
    }
}
