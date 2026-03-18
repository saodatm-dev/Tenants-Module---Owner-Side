using Didox.Application.Contracts.DidoxClient.Contracts.Documents.Act.Common;
using System.Text.Json.Serialization;

namespace Didox.Application.Contracts.DidoxClient.Contracts.Documents.Act.Request.Agents;

/// <summary>
/// Authorized person data.
/// </summary>
public class Agent
{
    /// <summary>
    /// Authorized person's job title.
    /// </summary>
    [JsonPropertyName("JobTitle")]
    public string? JobTitle { get; set; }

    /// <summary>
    /// Authorized person's full name.
    /// </summary>
    [JsonPropertyName("Fio")]
    public string? Fio { get; set; }

    /// <summary>
    /// Authorized person's passport data.
    /// </summary>
    [JsonPropertyName("Passport")]
    public Passport? Passport { get; set; }

    /// <summary>
    /// Authorized person's PINFL.
    /// </summary>
    [JsonPropertyName("AgentTin")]
    public string? AgentTin { get; set; }
}
