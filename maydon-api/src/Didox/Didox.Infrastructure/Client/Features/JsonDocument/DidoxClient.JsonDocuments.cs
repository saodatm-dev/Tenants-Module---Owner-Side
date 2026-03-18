using Didox.Infrastructure.Client.Extensions;
using Didox.Application.Contracts.DidoxClient.Contracts.CommonModules;
using Didox.Application.Contracts.DidoxClient.Contracts.Documents.Act.Request.Root;
using Didox.Application.Contracts.DidoxClient.Contracts.Documents.Act.Response.Root;
using Didox.Application.Contracts.DidoxClient.Contracts.Documents.CustomDocument.Request.Root;
using Didox.Application.Contracts.DidoxClient.Contracts.Documents.Factura.Request.Root;
using Didox.Application.Contracts.DidoxClient.Contracts.Documents.Factura.Response.Root;
using Didox.Application.Contracts.DidoxClient.Contracts.Documents.MultiClientDocument.Request.Root;
using Didox.Application.Contracts.DidoxClient.Contracts.Documents.MultiClientDocument.Response.Root;
using RestSharp;
// ReSharper disable CheckNamespace

namespace Didox.Infrastructure.Client.Features;

/// <summary>
/// Didox HTTP client for JSON document operations
/// </summary>
public partial class DidoxClient
{
    /// <inheritdoc/>
    public Task<DidoxApiResponse<PendingFacturaResponse>> CreateFacturaDocumentAsync(string documentId, C3FacturaRequest requestBody, string userToken, CancellationToken cancellationToken = default)
    {
        var url = _documentCreateUrl.Replace(":docType", DidoxDocType.InvoiceWithoutAct.GetEnumMemberValue());
        var request = CreateRequest(url, Method.Post)
            .AddHeader("user-key", userToken.Trim())
            .AddJsonBody(requestBody);
        
        return RestClient.ExecuteAsync(request, cancellationToken)
            .ContinueWith(t => ProcessResponse<PendingFacturaResponse>(t.Result), cancellationToken);
    }
    
    /// <inheritdoc/>
    public Task<DidoxApiResponse<EmpowermentDocumentResponse>> CreateActDocumentAsync(string documentId, EmpowermentRequest requestBody, string userToken, CancellationToken cancellationToken = default)
    {
        var url = _documentCreateUrl.Replace(":docType", DidoxDocType.ActOfCompletedWorks.GetEnumMemberValue());
        var request = CreateRequest(url, Method.Post)
            .AddHeader("user-key", userToken.Trim())
            .AddJsonBody(requestBody);
        
        return RestClient.ExecuteAsync(request, cancellationToken)
            .ContinueWith(t => ProcessResponse<EmpowermentDocumentResponse>(t.Result), cancellationToken);
    }
    
    /// <inheritdoc/>
    public Task<DidoxApiResponse<PendingFacturaResponse>> CreateCustomDocumentAsync(DocumentUploadRequest requestBody, string userToken, CancellationToken cancellationToken = default)
    {
        var url = _documentCreateUrl.Replace(":docType", DidoxDocType.CustomDocument.GetEnumMemberValue());
        var request = CreateRequest(url, Method.Post)
            .AddHeader("user-key", userToken.Trim())
            .AddJsonBody(requestBody);
        
        return RestClient.ExecuteAsync(request, cancellationToken)
            .ContinueWith(t => ProcessResponse<PendingFacturaResponse>(t.Result), cancellationToken);
    }
    
    /// <inheritdoc/>
    public Task<DidoxApiResponse<MultiClientDocumentResponse>> CreateMultiClientDocumentAsync(MultiClientDocumentUploadRequest requestBody, string userToken, CancellationToken cancellationToken = default)
    {
        var url = _documentCreateUrl.Replace(":docType", DidoxDocType.MultiPartyCustomDocument.GetEnumMemberValue());
        var request = CreateRequest(url, Method.Post)
            .AddHeader("user-key", userToken.Trim())
            .AddJsonBody(requestBody);
        
        return RestClient.ExecuteAsync(request, cancellationToken)
            .ContinueWith(t => ProcessResponse<MultiClientDocumentResponse>(t.Result), cancellationToken);
    }
}
