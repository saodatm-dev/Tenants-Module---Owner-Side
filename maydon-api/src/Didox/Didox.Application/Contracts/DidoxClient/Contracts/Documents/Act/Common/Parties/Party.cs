using System.Text.Json.Serialization;

namespace Didox.Application.Contracts.DidoxClient.Contracts.Documents.Act.Common.Parties;

/// <summary>
/// Transaction party data (buyer or seller).
/// </summary>
public class Party
{
    [JsonPropertyName("Name")]
    public string? Name { get; set; }

    [JsonPropertyName("BranchCode")]
    public string? BranchCode { get; set; }

    [JsonPropertyName("BranchName")]
    public string? BranchName { get; set; }

    [JsonPropertyName("Account")]
    public string? Account { get; set; }

    [JsonPropertyName("BankId")]
    public string? BankId { get; set; }

    [JsonPropertyName("Address")]
    public string? Address { get; set; }

    [JsonPropertyName("Director")]
    public string? Director { get; set; }

    [JsonPropertyName("Accountant")]
    public string? Accountant { get; set; }
    
}

