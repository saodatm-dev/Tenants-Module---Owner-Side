using System.Text.Json.Serialization;

namespace Didox.Application.Contracts.DidoxClient.Contracts.Documents.Act.Common.Docs;

/// <summary>
/// Power of attorney document.
/// </summary>
public class EmpowermentDoc
{
    /// <summary>
    /// Power of attorney number.
    /// </summary>
    [JsonPropertyName("EmpowermentNo")]
    public string? EmpowermentNo { get; set; }

    /// <summary>
    /// Power of attorney issue date.
    /// </summary>
    [JsonPropertyName("EmpowermentDateOfIssue")]
    public DateTime EmpowermentDateOfIssue { get; set; }

    /// <summary>
    /// Power of attorney expiration date.
    /// </summary>
    [JsonPropertyName("EmpowermentDateOfExpire")]
    public DateTime EmpowermentDateOfExpire { get; set; }
}

