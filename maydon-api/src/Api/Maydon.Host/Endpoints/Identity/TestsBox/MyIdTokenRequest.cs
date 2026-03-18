using System.Text.Json.Serialization;

namespace Maydon.Host.Endpoints.Identity.TestsBox;

public sealed record MyIdTokenRequest
{
    [JsonPropertyName("client_id")]
    public string ClientId { get; set; }
	
    [JsonPropertyName("client_secret")]
    public string ClientSecret { get; set; }
}