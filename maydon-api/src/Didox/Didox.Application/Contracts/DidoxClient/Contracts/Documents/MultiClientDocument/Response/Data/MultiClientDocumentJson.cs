using Didox.Application.Contracts.DidoxClient.Contracts.Documents.CommonModels;
using Didox.Application.Contracts.DidoxClient.Contracts.Documents.CustomDocument.Common;
using Didox.Application.Contracts.DidoxClient.Contracts.Documents.MultiClientDocument.Common;
using System.Text.Json.Serialization;

namespace Didox.Application.Contracts.DidoxClient.Contracts.Documents.MultiClientDocument.Response.Data;

/// <summary>
/// JSON structure of a multi-client document.
/// </summary>
public class MultiClientDocumentJson
{
    /// <summary>
    /// Unique Didox order identifier (order number).
    /// </summary>
    [JsonPropertyName("didoxorderid")]
    public string? DidoxOrderId { get; set; }

    /// <summary>
    /// Document metadata.
    /// </summary>
    [JsonPropertyName("document")]
    public DocumentInfo? Document { get; set; }

    /// <summary>
    /// Contract details associated with the document.
    /// </summary>
    [JsonPropertyName("contractdoc")]
    public ContractDoc? ContractDoc { get; set; }

    /// <summary>
    /// Document owner (contract initiator).
    /// </summary>
    [JsonPropertyName("owner")]
    public DocumentOwner? Owner { get; set; }

    /// <summary>
    /// List of document participants (clients).
    /// </summary>
    [JsonPropertyName("clients")]
    public List<DocumentClient>? Clients { get; set; }

    /// <summary>
    /// Unique identifier of the document.
    /// </summary>
    [JsonPropertyName("documentid")]
    public string? DocumentId { get; set; }

    /// <summary>
    /// URL for accessing or downloading the document.
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>
    /// Hash value of the document used for integrity verification.
    /// </summary>
    [JsonPropertyName("hash")]
    public string? Hash { get; set; }
}

