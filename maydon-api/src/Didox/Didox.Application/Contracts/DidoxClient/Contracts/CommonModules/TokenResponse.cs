using System.Text.Json.Serialization;

namespace Didox.Application.Contracts.DidoxClient.Contracts.CommonModules;

public class TokenResponse
{
    [JsonPropertyName("token")]
    public required string Token { get; set; }
}

