using Didox.Application.Contracts.DidoxClient.Contracts.CommonModules;
using Didox.Application.Contracts.DidoxClient.Contracts.Documents.Act.Request.Root;
using Didox.Application.Contracts.DidoxClient.Contracts.Documents.Act.Response.Root;
using Didox.Application.Contracts.DidoxClient.Contracts.Documents.CustomDocument.Request.Root;
using Didox.Application.Contracts.DidoxClient.Contracts.Documents.Factura.Request.Root;
using Didox.Application.Contracts.DidoxClient.Contracts.Documents.Factura.Response.Root;
using Didox.Application.Contracts.DidoxClient.Contracts.Documents.MultiClientDocument.Request.Root;
using Didox.Application.Contracts.DidoxClient.Contracts.Documents.MultiClientDocument.Response.Root;

namespace Didox.Application.Abstractions.Client.JsonDocument;

public interface IDidoxJsonDocumentClient
{
    /// <summary>
    /// Creates an electronic invoice (ESF) based on a JSON structure and registers the document in the Didox system.
    /// </summary>
    /// <param name="documentId">
    /// A unique document identifier on the client side, used to ensure request idempotency.
    /// </param>
    /// <param name="requestBody">
    /// Invoice data generated in accordance with the legislation of the Republic of Uzbekistan.
    /// </param>
    /// <param name="userToken">User's token from Didox system</param>
    /// <param name="cancellationToken">
    /// A token to cancel the asynchronous operation. Allows aborting the request execution.
    /// </param>
    /// <returns>
    /// The task result contains the Didox API response with the created document data in a pending status.
    /// </returns>
    Task<DidoxApiResponse<PendingFacturaResponse>> CreateFacturaDocumentAsync(string documentId, C3FacturaRequest requestBody,string userToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates an electronic act (empowerment document) based on a JSON struct and registers it in the Didox system.
    /// </summary>
    /// <param name="documentId">
    /// A unique document identifier on the client side, used to prevent duplicate requests.
    /// </param>
    /// <param name="requestBody">
    /// Empowerment data including contract details, authorized representative information, and a list of goods.
    /// </param>
    /// <param name="userToken">User's token from Didox system</param>
    /// <param name="cancellationToken">
    /// A token to cancel the asynchronous operation. Used to terminate the request prematurely.
    /// </param>
    /// <returns>
    /// The task result contains the Didox API response with the JSON representation of the created act.
    /// </returns>
    Task<DidoxApiResponse<EmpowermentDocumentResponse>> CreateActDocumentAsync(string documentId, EmpowermentRequest requestBody, string userToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uploads an arbitrary user document (for example, a PDF file) along with metadata and registers it in the Didox system.
    /// </summary>
    /// <param name="requestBody">
    /// Document data and file content encoded in Base64 (data:application/pdf;base64,...).
    /// </param>
    /// <param name="userToken">User's token in didox</param>
    /// <param name="cancellationToken">
    /// A token to cancel the asynchronous operation. Allows aborting the HTTP request execution.
    /// </param>
    /// <returns>
    /// The task result contains the Didox API response with information about the registered document and a download link.
    /// </returns>
    Task<DidoxApiResponse<PendingFacturaResponse>> CreateCustomDocumentAsync(DocumentUploadRequest requestBody, string userToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates and registers a document with multiple recipients (clients) based on a JSON structure and an attached PDF file.
    /// </summary>
    /// <param name="requestBody">
    /// Document data including metadata, owner information, a list of recipients (clients), and the document file encoded in Base64 (<c>data:application/pdf;base64,...</c>).
    /// </param>
    /// <param name="userToken">User's token from didox system</param>
    /// <param name="cancellationToken">
    /// A token to cancel the asynchronous operation. Allows aborting the HTTP request if necessary.
    /// </param>
    /// <returns>
    /// The task result contains the Didox API response with information about the registered document, its unique identifier, and a download link for the file.
    /// </returns>
    Task<DidoxApiResponse<MultiClientDocumentResponse>> CreateMultiClientDocumentAsync(MultiClientDocumentUploadRequest requestBody, string userToken, CancellationToken cancellationToken = default);
        
}
