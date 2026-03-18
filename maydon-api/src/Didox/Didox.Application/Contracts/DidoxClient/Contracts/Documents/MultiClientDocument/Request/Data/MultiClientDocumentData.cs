using Didox.Application.Contracts.DidoxClient.Contracts.Documents.CommonModels;
using Didox.Application.Contracts.DidoxClient.Contracts.Documents.CustomDocument.Common;
using Didox.Application.Contracts.DidoxClient.Contracts.Documents.MultiClientDocument.Common;
using System.Text.Json.Serialization;

namespace Didox.Application.Contracts.DidoxClient.Contracts.Documents.MultiClientDocument.Request.Data;

/// <summary>
/// Container with document data, owner, and list of clients.
/// </summary>
public class MultiClientDocumentData
{
    /// <summary>
    /// Basic document information.
    /// </summary>
    [JsonPropertyName("Document")]
    public DocumentInfo? Document { get; set; }

    /// <summary>
    /// Contract data associated with the document.
    /// </summary>
    [JsonPropertyName("ContractDoc")]
    public ContractDoc? ContractDoc { get; set; }

    /// <summary>
    /// Document owner (supplier).
    /// </summary>
    [JsonPropertyName("Owner")]
    public DocumentOwner? Owner { get; set; }

    /// <summary>
    /// List of clients (buyers) participating in the document.
    /// </summary>
    [JsonPropertyName("Clients")]
    public List<DocumentClient>? Clients { get; set; }
}

