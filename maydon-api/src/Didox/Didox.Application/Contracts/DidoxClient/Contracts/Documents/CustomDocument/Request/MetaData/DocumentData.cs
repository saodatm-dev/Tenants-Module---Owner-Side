using Didox.Application.Contracts.DidoxClient.Contracts.Documents.CommonModels;
using Didox.Application.Contracts.DidoxClient.Contracts.Documents.CustomDocument.Common;
using System.Text.Json.Serialization;

namespace Didox.Application.Contracts.DidoxClient.Contracts.Documents.CustomDocument.Request.MetaData;

/// <summary>
/// Container for document metadata.
/// </summary>
public class DocumentData
{
    /// <summary>
    /// General information about the document.
    /// </summary>
    [JsonPropertyName("Document")]
    public DocumentInfo? Document { get; set; }

    /// <summary>
    /// Document subtype.
    /// The value is defined by the system’s business logic.
    /// </summary>
    [JsonPropertyName("Subtype")]
    public int Subtype { get; set; }

    /// <summary>
    /// Contract data (if available).
    /// </summary>
    [JsonPropertyName("ContractDoc")]
    public ContractDoc? ContractDoc { get; set; }

    /// <summary>
    /// Seller's TIN or PINFL.
    /// </summary>
    [JsonPropertyName("SellerTin")]
    public string? SellerTin { get; set; }

    /// <summary>
    /// Seller information.
    /// </summary>
    [JsonPropertyName("Seller")]
    public SimpleParty? Seller { get; set; }

    /// <summary>
    /// Buyer's TIN or PINFL.
    /// </summary>
    [JsonPropertyName("BuyerTin")]
    public string? BuyerTin { get; set; }

    /// <summary>
    /// Buyer information.
    /// </summary>
    [JsonPropertyName("Buyer")]
    public SimpleParty? Buyer { get; set; }
}

