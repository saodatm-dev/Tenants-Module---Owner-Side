using System.Text.Json.Serialization;

namespace Didox.Application.Contracts.DidoxClient.Contracts.Documents.Factura.Request.Parties;

// Seller / Buyer
public class Party
{
    public string? VatRegCode { get; set; }
    public string? Account { get; set; }
    public string? BankId { get; set; }
    public string? Director { get; set; }
    public string? Accountant { get; set; }
    public int VatRegStatus { get; set; }
    
    /// <summary>
    /// Organization name.
    /// </summary>
    [JsonPropertyName("Name")]
    public string? Name { get; set; }

    /// <summary>
    /// Branch code.
    /// </summary>
    [JsonPropertyName("BranchCode")]
    public string? BranchCode { get; set; }

    /// <summary>
    /// Branch name.
    /// </summary>
    [JsonPropertyName("BranchName")]
    public string? BranchName { get; set; }

    /// <summary>
    /// Registered address.
    /// </summary>
    [JsonPropertyName("Address")]
    public string? Address { get; set; }
}

