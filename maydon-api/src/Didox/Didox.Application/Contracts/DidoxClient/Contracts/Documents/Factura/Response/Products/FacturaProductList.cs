using System.Text.Json.Serialization;

namespace Didox.Application.Contracts.DidoxClient.Contracts.Documents.Factura.Response.Products;

/// <summary>
/// List of goods (services) specified in the invoice-factura.
/// </summary>
public class FacturaProductList
{
    [JsonPropertyName("tin")]
    public string? Tin { get; set; }

    [JsonPropertyName("hasvat")]
    public bool HasVat { get; set; }

    [JsonPropertyName("products")]
    public List<FacturaProductItem>? Products { get; set; }

    /// <summary>
    /// Invoice-factura product list identifier.
    /// </summary>
    [JsonPropertyName("facturaproductid")]
    public string? FacturaProductId { get; set; }
}

