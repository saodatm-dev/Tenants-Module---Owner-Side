using Didox.Application.Contracts.DidoxClient.Contracts.Documents.Factura.Response.Documents;
using System.Text.Json.Serialization;

namespace Didox.Application.Contracts.DidoxClient.Contracts.Documents.Factura.Response.Root;

/// <summary>
/// Response model for a pending factura document returned by the Didox API.
/// </summary>
public class PendingFacturaResponse
{
    /// <summary>
    /// Container with the pending factura document JSON.
    /// </summary>
    [JsonPropertyName("pending_document")]
    public PendingDocument? PendingDocument { get; set; }

    /// <summary>
    /// Unique identifier of the pending document in Didox.
    /// </summary>
    [JsonPropertyName("_id")]
    public string? Id { get; set; }

    /// <summary>
    /// Date and time when the document was created.
    /// </summary>
    [JsonPropertyName("created_date")]
    public string? CreatedDate { get; set; }
}

