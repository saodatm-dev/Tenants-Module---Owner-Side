using System.Text.Json.Serialization;

namespace Didox.Application.Contracts.DidoxClient.Contracts.Documents.Act.Request.Products;

/// <summary>
/// List of goods transferred under the power of attorney.
/// </summary>
public class EmpowermentProductList
{
    /// <summary>
    /// Seller's TIN or PINFL.
    /// </summary>
    [JsonPropertyName("Tin")]
    public string? Tin { get; set; }

    /// <summary>
    /// Flag indicating presence of excise goods.
    /// </summary>
    [JsonPropertyName("HasExcise")]
    public bool HasExcise { get; set; }

    /// <summary>
    /// Flag indicating VAT application.
    /// </summary>
    [JsonPropertyName("HasVat")]
    public bool HasVat { get; set; }

    /// <summary>
    /// List of products.
    /// </summary>
    [JsonPropertyName("Products")]
    public List<EmpowermentProductItem>? Products { get; set; }
}

