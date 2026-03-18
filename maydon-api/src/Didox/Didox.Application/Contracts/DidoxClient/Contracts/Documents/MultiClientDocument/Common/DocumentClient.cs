using System.Text.Json.Serialization;

namespace Didox.Application.Contracts.DidoxClient.Contracts.Documents.MultiClientDocument.Common;

/// <summary>
/// Client (buyer) participating in the document.
/// </summary>
public class DocumentClient
{
    /// <summary>
    /// Buyer's TIN or PINFL.
    /// </summary>
    [JsonPropertyName("Tin")]
    public string? Tin { get; set; }

    /// <summary>
    /// Buyer's name.
    /// </summary>
    [JsonPropertyName("Name")]
    public string? Name { get; set; }

    /// <summary>
    /// Buyer's address.
    /// </summary>
    [JsonPropertyName("Address")]
    public string? Address { get; set; }
}

