using Didox.Application.Contracts.DidoxClient.Contracts.Documents.CustomDocument.Request.MetaData;
using System.Text.Json.Serialization;

namespace Didox.Application.Contracts.DidoxClient.Contracts.Documents.CustomDocument.Request.Root;

/// <summary>
/// Custom documents upload request
/// </summary>
public class DocumentUploadRequest
{
    /// <summary>
    /// Document details
    /// </summary>
    [JsonPropertyName("data")]
    public DocumentData? Data { get; set; }

    /// <summary>
    /// PDF-File represented in Base64: data:application/pdf;base64, 
    /// </summary>
    [JsonPropertyName("document")]
    public string? Document { get; set; }
}

