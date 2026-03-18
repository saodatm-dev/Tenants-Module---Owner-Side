using System.Text.Json.Serialization;

namespace Maydon.Host.Endpoints.Identity.TestsBox;

public sealed record MyIdSessionResponse()
{
    [JsonPropertyName("session_id")]
    public string SessionId { get; set; }
}