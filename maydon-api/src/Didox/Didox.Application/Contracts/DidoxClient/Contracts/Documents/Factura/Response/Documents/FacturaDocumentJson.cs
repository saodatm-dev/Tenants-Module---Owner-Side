using Didox.Application.Contracts.DidoxClient.Contracts.Documents.CommonModels;
using Didox.Application.Contracts.DidoxClient.Contracts.Documents.Factura.Commons.Documents;
using Didox.Application.Contracts.DidoxClient.Contracts.Documents.Factura.Response.Parties;
using Didox.Application.Contracts.DidoxClient.Contracts.Documents.Factura.Response.Products;
using System.Text.Json.Serialization;

namespace Didox.Application.Contracts.DidoxClient.Contracts.Documents.Factura.Response.Documents;

/// <summary>
/// JSON-Document from Didox represent Factura.
/// </summary>
public class FacturaDocumentJson
{
    [JsonPropertyName("version")]
    public int Version { get; set; }

    [JsonPropertyName("facturatype")]
    public int FacturaType { get; set; }

    [JsonPropertyName("productlist")]
    public FacturaProductList? ProductList { get; set; }

    [JsonPropertyName("facturadoc")]
    public FacturaDoc? FacturaDoc { get; set; }

    [JsonPropertyName("contractdoc")]
    public ContractDoc? ContractDoc { get; set; }

    [JsonPropertyName("sellertin")]
    public string? SellerTin { get; set; }

    [JsonPropertyName("seller")]
    public FacturaParty? Seller { get; set; }

    [JsonPropertyName("buyertin")]
    public string? BuyerTin { get; set; }

    [JsonPropertyName("buyer")]
    public FacturaParty? Buyer { get; set; }

    [JsonPropertyName("facturaempowermentdoc")]
    public FacturaEmpowermentDocResponse? FacturaEmpowermentDoc { get; set; }

    /// <summary>
    /// Factura unique identifier.
    /// </summary>
    [JsonPropertyName("facturaid")]
    public string? FacturaId { get; set; }

    [JsonPropertyName("hasrent")]
    public bool HasRent { get; set; }

    [JsonPropertyName("facturarentdoc")]
    public object? FacturaRentDoc { get; set; }

    [JsonPropertyName("contractid")]
    public string? ContractId { get; set; }
}

