using System.Text.Json.Serialization;

namespace Didox.Application.Contracts.DidoxClient.Contracts.Documents.Act.Common;

/// <summary>
/// Passport data of an individual.
/// </summary>
public class Passport
{
    /// <summary>
    /// Passport series and number.
    /// </summary>
    [JsonPropertyName("Number")]
    public string? Number { get; set; }

    /// <summary>
    /// Authority that issued the passport.
    /// </summary>
    [JsonPropertyName("IssuedBy")]
    public string? IssuedBy { get; set; }

    /// <summary>
    /// Passport issue date.
    /// </summary>
    [JsonPropertyName("DateOfIssue")]
    public DateTime? DateOfIssue { get; set; }
}

