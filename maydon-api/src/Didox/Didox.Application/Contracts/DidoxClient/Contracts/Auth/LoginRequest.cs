using System.Text.Json.Serialization;

namespace Didox.Application.Contracts.DidoxClient.Contracts.Auth;

/// <summary>
/// Request model for login
/// </summary>
public record LoginRequest
{
    [JsonPropertyName("password")]
    public string? Password { get; init; }
    
    [JsonPropertyName("signature")]
    public string? Signature { get; init; }
}


