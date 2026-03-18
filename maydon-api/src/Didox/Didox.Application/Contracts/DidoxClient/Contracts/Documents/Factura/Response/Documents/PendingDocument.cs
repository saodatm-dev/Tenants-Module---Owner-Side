using System.Text.Json.Serialization;

namespace Didox.Application.Contracts.DidoxClient.Contracts.Documents.Factura.Response.Documents;

/// <summary>
/// Pending document container.
/// </summary>
public class PendingDocument
{
    /// <summary>
    /// JSON representation of the invoice-factura.
    /// </summary>
    [JsonPropertyName("document_json")]
    public FacturaDocumentJson? DocumentJson { get; set; }
}

