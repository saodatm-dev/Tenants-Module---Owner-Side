using Didox.Application.Contracts.DidoxClient.Contracts.Documents.MultiClientDocument.Request.Data;
using System.Text.Json.Serialization;

namespace Didox.Application.Contracts.DidoxClient.Contracts.Documents.MultiClientDocument.Request.Root;

/// <summary>
/// Request for uploading a document related to an agreement (contract)
/// in PDF format encoded as Base64.
/// </summary>
public class MultiClientDocumentUploadRequest
{
    /// <summary>
    /// Metadata of the document being uploaded.
    /// </summary>
    [JsonPropertyName("data")]
    public MultiClientDocumentData? Data { get; set; }

    /// <summary>
    /// PDF document encoded in Base64.
    /// Example: data:application/pdf;base64,...
    /// </summary>
    [JsonPropertyName("document")]
    public string? Document { get; set; }
}

