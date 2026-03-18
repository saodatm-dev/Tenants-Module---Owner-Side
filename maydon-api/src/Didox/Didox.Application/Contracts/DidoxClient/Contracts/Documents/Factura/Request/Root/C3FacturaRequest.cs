using Didox.Application.Contracts.DidoxClient.Contracts.Documents.CommonModels;
using Didox.Application.Contracts.DidoxClient.Contracts.Documents.Factura.Commons.Documents;
using Didox.Application.Contracts.DidoxClient.Contracts.Documents.Factura.Request.AdditionalData;
using Didox.Application.Contracts.DidoxClient.Contracts.Documents.Factura.Request.Parties;
using Didox.Application.Contracts.DidoxClient.Contracts.Documents.Factura.Request.Products;
using System.Text.Json.Serialization;

namespace Didox.Application.Contracts.DidoxClient.Contracts.Documents.Factura.Request.Root;

public class C3FacturaRequest
{
    [JsonPropertyName("Version")]
    public int Version { get; set; }

    [JsonPropertyName("WaybillLocalIds")]
    public List<string>? WaybillLocalIds { get; set; }

    [JsonPropertyName("HasMarking")]
    public bool HasMarking { get; set; }

    [JsonPropertyName("HasRent")]
    public bool HasRent { get; set; }

    [JsonPropertyName("FacturaRentDoc")]
    public object? FacturaRentDoc { get; set; }

    [JsonPropertyName("FacturaType")]
    public int FacturaType { get; set; }

    [JsonPropertyName("ProductList")]
    public ProductList? ProductList { get; set; }

    [JsonPropertyName("FacturaDoc")]
    public FacturaDoc? FacturaDoc { get; set; }

    [JsonPropertyName("ContractDoc")]
    public ContractDoc? ContractDoc { get; set; }

    [JsonPropertyName("ContractId")]
    public string? ContractId { get; set; }

    [JsonPropertyName("LotId")]
    public string? LotId { get; set; }

    [JsonPropertyName("OldFacturaDoc")]
    public OldFacturaDoc? OldFacturaDoc { get; set; }

    [JsonPropertyName("SellerTin")]
    public string? SellerTin { get; set; }

    [JsonPropertyName("Seller")]
    public Party? Seller { get; set; }

    [JsonPropertyName("ItemReleasedDoc")]
    public ItemReleasedDoc? ItemReleasedDoc { get; set; }

    [JsonPropertyName("BuyerTin")]
    public string? BuyerTin { get; set; }

    [JsonPropertyName("Buyer")]
    public Party? Buyer { get; set; }

    [JsonPropertyName("FacturaInvestmentObjectDoc")]
    public FacturaInvestmentObjectDoc? FacturaInvestmentObjectDoc { get; set; }

    [JsonPropertyName("FacturaEmpowermentDoc")]
    public FacturaEmpowermentDoc? FacturaEmpowermentDoc { get; set; }

    [JsonPropertyName("ForeignCompany")]
    public ForeignCompany? ForeignCompany { get; set; }
}

