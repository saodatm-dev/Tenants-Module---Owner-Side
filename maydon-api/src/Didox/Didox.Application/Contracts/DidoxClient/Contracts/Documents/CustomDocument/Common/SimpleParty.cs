using System.Text.Json.Serialization;

namespace Didox.Application.Contracts.DidoxClient.Contracts.Documents.CustomDocument.Common;

/// <summary>
/// Simplified transaction party data
/// (supplier or buyer).
/// </summary>
public class SimpleParty
{
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
    /// Legal address.
    /// </summary>
    [JsonPropertyName("Address")]
    public string? Address { get; set; }
}

