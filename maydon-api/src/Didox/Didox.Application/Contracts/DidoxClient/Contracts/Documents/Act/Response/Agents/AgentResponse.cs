using Didox.Application.Contracts.DidoxClient.Contracts.Documents.Act.Common;
using System.Text.Json.Serialization;

namespace Didox.Application.Contracts.DidoxClient.Contracts.Documents.Act.Response.Agents;

/// <summary>
/// Authorized person data.
/// </summary>
public class AgentResponse
{
    [JsonPropertyName("jobtitle")]
    public string? JobTitle { get; set; }

    [JsonPropertyName("fio")]
    public string? Fio { get; set; }

    [JsonPropertyName("passport")]
    public Passport? Passport { get; set; }

    [JsonPropertyName("agenttin")]
    public string? AgentTin { get; set; }

    /// <summary>
    /// Authorized person identifier in the system.
    /// </summary>
    [JsonPropertyName("agentempowermentid")]
    public string? AgentEmpowermentId { get; set; }
}

