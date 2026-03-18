using System.Text.Json.Serialization;

namespace Didox.Application.Contracts.DidoxClient.Contracts.Documents.Factura.Response.Products;

/// <summary>
/// Product (or service) item in the invoice-factura.
/// </summary>
public class FacturaProductItem
{
    [JsonPropertyName("ordno")]
    public int OrdNo { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("catalogcode")]
    public string? CatalogCode { get; set; }

    [JsonPropertyName("catalogname")]
    public string? CatalogName { get; set; }

    [JsonPropertyName("packagecode")]
    public string? PackageCode { get; set; }

    [JsonPropertyName("packagename")]
    public string? PackageName { get; set; }

    [JsonPropertyName("count")]
    public string? Count { get; set; }

    [JsonPropertyName("summa")]
    public string? Summa { get; set; }

    [JsonPropertyName("deliverysum")]
    public string? DeliverySum { get; set; }

    [JsonPropertyName("vatrate")]
    public string? VatRate { get; set; }

    [JsonPropertyName("vatsum")]
    public string? VatSum { get; set; }

    [JsonPropertyName("deliverysumwithvat")]
    public string? DeliverySumWithVat { get; set; }

    [JsonPropertyName("withoutvat")]
    public bool WithoutVat { get; set; }

    [JsonPropertyName("origin")]
    public int Origin { get; set; }

    [JsonPropertyName("committentvatregstatus")]
    public int? CommittentVatRegStatus { get; set; }

    [JsonPropertyName("marks")]
    public string? Marks { get; set; }

    [JsonPropertyName("lgotaid")]
    public string? LgotaId { get; set; }

    [JsonPropertyName("lgotaname")]
    public string? LgotaName { get; set; }

    [JsonPropertyName("lgotavatsum")]
    public decimal LgotaVatSum { get; set; }

    [JsonPropertyName("lgotatype")]
    public int? LgotaType { get; set; }
}

