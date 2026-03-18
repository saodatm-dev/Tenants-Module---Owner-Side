using System.Text.Json.Serialization;

namespace Didox.Application.Contracts.DidoxClient.Contracts.Documents.Factura.Response.Documents;

/// <summary>
/// Power of attorney data used when signing the invoice-factura.
/// </summary>
public class FacturaEmpowermentDocResponse
{
    [JsonPropertyName("empowermentno")]
    public string? EmpowermentNo { get; set; }

    [JsonPropertyName("empowermentdateofissue")]
    public string? EmpowermentDateOfIssue { get; set; }

    [JsonPropertyName("agentfio")]
    public string? AgentFio { get; set; }

    [JsonPropertyName("agenttin")]
    public string? AgentTin { get; set; }

    /// <summary>
    /// Power of attorney identifier in the system.
    /// </summary>
    [JsonPropertyName("agentfacturaid")]
    public string? AgentFacturaId { get; set; }
}

