using Didox.Application.Contracts.DidoxClient.Contracts.Documents.Act.Request.Products;
using System.Text.Json.Serialization;

namespace Didox.Application.Contracts.DidoxClient.Contracts.Documents.Act.Response.Products;

/// <summary>
/// List of goods specified in the power of attorney.
/// </summary>
public class EmpowermentProductListResponse
{
    /// <summary>
    /// TIN or PINFL.
    /// </summary>
    [JsonPropertyName("tin")]
    public string? Tin { get; set; }
    
    /// <summary>
    /// Flag indicating presence of excise goods.
    /// </summary>
    [JsonPropertyName("hasexcise")]
    public bool HasExcise { get; set; }
    
    /// <summary>
    /// Flag indicating VAT application.
    /// </summary>
    [JsonPropertyName("hasvat")]
    public bool HasVat { get; set; }
    
    /// <summary>
    /// List of products.
    /// </summary>
    [JsonPropertyName("products")]
    public List<EmpowermentProductItem>? Products { get; set; }

    /// <summary>
    /// Power of attorney product list identifier.
    /// </summary>
    [JsonPropertyName("empowermentproductid")]
    public string? EmpowermentProductId { get; set; }
}

