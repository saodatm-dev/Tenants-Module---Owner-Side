using System.Text.Json.Serialization;

namespace Didox.Application.Contracts.DidoxClient.Contracts.Documents.Factura.Commons.Documents;

public class FacturaDoc
{
    [JsonPropertyName("facturano")]
    public string? FacturaNo { get; set; }

    [JsonPropertyName("facturadate")]
    public string? FacturaDate { get; set; }
}

