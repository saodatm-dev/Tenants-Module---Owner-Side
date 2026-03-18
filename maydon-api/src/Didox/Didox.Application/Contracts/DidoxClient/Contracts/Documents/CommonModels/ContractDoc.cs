using System.Text.Json.Serialization;

namespace Didox.Application.Contracts.DidoxClient.Contracts.Documents.CommonModels;

/// <summary>
/// Contract data on the basis of which the power of attorney was issued.
/// </summary>
public class ContractDoc
{
    /// <summary>
    /// Contract number.
    /// </summary>
    [JsonPropertyName("contractno")]
    public string? ContractNo { get; set; }

    /// <summary>
    /// Contract date.
    /// </summary>
    [JsonPropertyName("contractdate")]
    public string? ContractDate { get; set; }
}

