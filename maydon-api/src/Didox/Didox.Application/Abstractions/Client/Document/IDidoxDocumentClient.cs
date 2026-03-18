using System.Text.Json;
using Didox.Application.Contracts.DidoxClient.Contracts.CommonModules;

namespace Didox.Application.Abstractions.Client.Document;

public interface IDidoxDocumentClient
{
    /// <summary>
    /// Retrieves the document content in HTML format.
    /// </summary>
    /// <param name="documentId">
    /// The unique identifier of the document to retrieve.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// The task result contains a <see cref="DidoxApiResponse{T}"/> with the document content as an HTML string if the request succeeds.
    /// </returns>
    /// <remarks>
    /// This method returns the raw HTML representation of the document without performing any additional processing or transformation.
    /// </remarks>
    public Task<DidoxApiResponse<string>> GetHtmlDocumentAsync(string documentId, CancellationToken cancellationToken = default);


    /// <summary>
    /// Retrieves the document content in PDF format.
    /// </summary>
    /// <param name="documentId">
    /// The unique identifier of the document to retrieve.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a <see cref="DidoxApiResponse{T}"/> with the document content as a byte array representing a PDF file.
    /// </returns>
    /// <remarks>
    /// The returned byte array can be persisted to storage, streamed to a client, or processed further as needed.
    /// </remarks>
    public Task<DidoxApiResponse<byte[]>> GetPdfDocumentAsync(string documentId, CancellationToken cancellationToken = default);


    /// <summary>
    /// Retrieves the document content as a JSON document without deserializing it into a model.
    /// </summary>
    /// <param name="documentId">
    /// The unique identifier of the document to retrieve.
    /// </param>
    /// <param name="userToken">User's auth token in Didox</param>
    /// <param name="cancellationToken">
    /// A token to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a <see cref="DidoxApiResponse{T}"/> with a
    /// <see cref="JsonDocument"/> representing the raw JSON payload.
    /// </returns>
    /// <remarks>
    /// This method is intended for scenarios where the document schema is dynamic, unknown, or only partially required. The returned <see cref="JsonDocument"/>
    /// allows safe, structured access to the JSON data without full deserialization.
    /// The caller is responsible for disposing the returned <see cref="JsonDocument"/> when it is no longer needed.
    /// </remarks>
    public Task<DidoxApiResponse<System.Text.Json.JsonDocument>> GetDidoxJsonDoc(string documentId, string userToken, CancellationToken cancellationToken = default);
        
}
