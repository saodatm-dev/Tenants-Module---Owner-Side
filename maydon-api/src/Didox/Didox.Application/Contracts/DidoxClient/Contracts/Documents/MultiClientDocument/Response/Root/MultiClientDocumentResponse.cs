using Didox.Application.Contracts.DidoxClient.Contracts.Documents.MultiClientDocument.Response.Data;
using System.Text.Json.Serialization;

namespace Didox.Application.Contracts.DidoxClient.Contracts.Documents.MultiClientDocument.Response.Root;

/// <summary>
/// API response containing information about a multi-client document
/// with detailed document data.
/// </summary>
public class MultiClientDocumentResponse
{
    /// <summary>
    /// JSON representation of the document returned by the Didox service.
    /// </summary>
    [JsonPropertyName("document_json")]
    public MultiClientDocumentJson? DocumentJson { get; set; }
}

