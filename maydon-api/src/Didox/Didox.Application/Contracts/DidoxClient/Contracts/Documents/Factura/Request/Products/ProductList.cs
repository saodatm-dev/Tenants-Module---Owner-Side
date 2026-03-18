using System.Text.Json.Serialization;

namespace Didox.Application.Contracts.DidoxClient.Contracts.Documents.Factura.Request.Products;

public class ProductList
{
    [JsonPropertyName("HasCommittent")]
    public bool HasCommittent { get; set; }

    [JsonPropertyName("HasLgota")]
    public bool HasLgota { get; set; }

    [JsonPropertyName("Tin")]
    public string? Tin { get; set; }

    [JsonPropertyName("HasExcise")]
    public bool HasExcise { get; set; }

    [JsonPropertyName("HasVat")]
    public bool HasVat { get; set; }

    [JsonPropertyName("Products")]
    public List<ProductItem>? Products { get; set; }
}

