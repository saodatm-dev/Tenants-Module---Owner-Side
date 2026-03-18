using System.Text.Json.Serialization;

namespace Didox.Application.Contracts.DidoxClient.Contracts.Documents.Act.Request.Products;

/// <summary>
/// Product item in the power of attorney.
/// </summary>
public class EmpowermentProductItem
{
    /// <summary>
    /// Item sequence number.
    /// </summary>
    [JsonPropertyName("OrdNo")]
    public int OrdNo { get; set; }

    /// <summary>
    /// Catalog code (IKPU).
    /// </summary>
    [JsonPropertyName("CatalogCode")]
    public string? CatalogCode { get; set; }

    /// <summary>
    /// Catalog name (IKPU).
    /// </summary>
    [JsonPropertyName("CatalogName")]
    public string? CatalogName { get; set; }

    /// <summary>
    /// Product name.
    /// </summary>
    [JsonPropertyName("Name")]
    public string? Name { get; set; }

    /// <summary>
    /// Unit of measure.
    /// </summary>
    [JsonPropertyName("MeasureId")]
    public string? MeasureId { get; set; }

    /// <summary>
    /// Product quantity.
    /// </summary>
    [JsonPropertyName("Count")]
    public string? Count { get; set; }
}

