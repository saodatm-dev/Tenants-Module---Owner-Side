using Didox.Application.Contracts.DidoxClient.Contracts.Documents.Act.Common.Docs;
using Didox.Application.Contracts.DidoxClient.Contracts.Documents.Act.Common.Parties;
using Didox.Application.Contracts.DidoxClient.Contracts.Documents.Act.Response.Agents;
using Didox.Application.Contracts.DidoxClient.Contracts.Documents.Act.Response.Products;
using Didox.Application.Contracts.DidoxClient.Contracts.Documents.CommonModels;
using System.Text.Json.Serialization;

namespace Didox.Application.Contracts.DidoxClient.Contracts.Documents.Act.Response.Root;

/// <summary>
/// API response containing the JSON representation of an empowerment document.
/// </summary>
public class EmpowermentDocumentResponse
{
    /// <summary>
    /// JSON data of the generated empowerment document.
    /// </summary>
    [JsonPropertyName("document_json")]
    public EmpowermentDocumentJson? DocumentJson { get; set; }
}

/// <summary>
/// JSON structure of the empowerment document.
/// </summary>
public class EmpowermentDocumentJson
{
    /// <summary>
    /// Empowerment document details.
    /// </summary>
    [JsonPropertyName("empowermentdoc")]
    public EmpowermentDoc? EmpowermentDoc { get; set; }
    
    /// <summary>
    /// Contract document details.
    /// </summary>
    [JsonPropertyName("contractdoc")]
    public ContractDoc? ContractDoc { get; set; }
    
    /// <summary>
    /// Agent information associated with the empowerment.
    /// </summary>
    [JsonPropertyName("agent")]
    public AgentResponse? Agent { get; set; }
    
    /// <summary>
    /// Seller's Tax Identification Number (TIN).
    /// </summary>
    [JsonPropertyName("sellertin")]
    public string? SellerTin { get; set; }
    
    /// <summary>
    /// Seller information.
    /// </summary>
    [JsonPropertyName("seller")]
    public Party? Seller { get; set; }
    
    /// <summary>
    /// Buyer's Tax Identification Number (TIN).
    /// </summary>
    [JsonPropertyName("buyertin")]
    public string? BuyerTin { get; set; }
    
    /// <summary>
    /// List of products included in the empowerment.
    /// </summary>
    [JsonPropertyName("productlist")]
    public EmpowermentProductListResponse? ProductList { get; set; }
    
    /// <summary>
    /// Buyer information.
    /// </summary>
    [JsonPropertyName("buyer")]
    public Party? Buyer { get; set; }

    /// <summary>
    /// Unique identifier of the created empowerment document.
    /// </summary>
    [JsonPropertyName("empowermentid")]
    public string? EmpowermentId { get; set; }
}

