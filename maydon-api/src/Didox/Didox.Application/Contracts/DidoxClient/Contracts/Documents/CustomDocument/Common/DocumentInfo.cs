using System.Text.Json.Serialization;

namespace Didox.Application.Contracts.DidoxClient.Contracts.Documents.CustomDocument.Common;

/// <summary>
/// Basic document information.
/// </summary>
public class DocumentInfo
{
    /// <summary>
    /// Document number.
    /// </summary>
    [JsonPropertyName("DocumentNo")]
    public string? DocumentNo { get; set; }

    /// <summary>
    /// Document date.
    /// </summary>
    [JsonPropertyName("DocumentDate")]
    public string? DocumentDate { get; set; }

    /// <summary>
    /// Document name.
    /// </summary>
    [JsonPropertyName("DocumentName")]
    public string? DocumentName { get; set; }
}

