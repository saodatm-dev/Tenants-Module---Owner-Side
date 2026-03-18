using System.Text.Json.Serialization;

namespace Didox.Application.Contracts.DidoxClient.Contracts.Documents.MultiClientDocument.Common;

/// <summary>
/// Document owner (supplier).
/// </summary>
public class DocumentOwner
{
    /// <summary>
    /// Supplier's TIN or PINFL.
    /// </summary>
    [JsonPropertyName("Tin")]
    public string? Tin { get; set; }

    /// <summary>
    /// Supplier's name.
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
    /// Supplier's address.
    /// </summary>
    [JsonPropertyName("Address")]
    public string? Address { get; set; }
}

