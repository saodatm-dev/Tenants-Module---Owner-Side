using Didox.Application.Contracts.DidoxClient.Contracts.Documents.Act.Common.Docs;
using Didox.Application.Contracts.DidoxClient.Contracts.Documents.Act.Request.Agents;
using Didox.Application.Contracts.DidoxClient.Contracts.Documents.Act.Request.Products;
using Didox.Application.Contracts.DidoxClient.Contracts.Documents.CommonModels;
using System.Text.Json.Serialization;
using Parties_Party = Didox.Application.Contracts.DidoxClient.Contracts.Documents.Act.Common.Parties.Party;

namespace Didox.Application.Contracts.DidoxClient.Contracts.Documents.Act.Request.Root;

/// <summary>
/// Request for creating an empowerment document and attaching it to a contract.
/// </summary>
public class EmpowermentRequest
{
    /// <summary>
    /// Empowerment document.
    /// </summary>
    [JsonPropertyName("EmpowermentDoc")]
    public EmpowermentDoc? EmpowermentDoc { get; set; }

    /// <summary>
    /// Contract document.
    /// </summary>
    [JsonPropertyName("ContractDoc")]
    public ContractDoc? ContractDoc { get; set; }

    /// <summary>
    /// Agent information.
    /// </summary>
    [JsonPropertyName("Agent")]
    public Agent? Agent { get; set; }

    /// <summary>
    /// Seller's Tax Identification Number (TIN).
    /// </summary>
    [JsonPropertyName("SellerTin")]
    public string? SellerTin { get; set; }

    /// <summary>
    /// Seller information.
    /// </summary>
    [JsonPropertyName("Seller")]
    public Parties_Party? Seller { get; set; }

    /// <summary>
    /// Buyer's Tax Identification Number (TIN).
    /// </summary>
    [JsonPropertyName("BuyerTin")]
    public string? BuyerTin { get; set; }

    /// <summary>
    /// List of products covered by the empowerment.
    /// </summary>
    [JsonPropertyName("ProductList")]
    public EmpowermentProductList? ProductList { get; set; }

    /// <summary>
    /// Buyer information.
    /// </summary>
    [JsonPropertyName("Buyer")]
    public Parties_Party? Buyer { get; set; }
}

