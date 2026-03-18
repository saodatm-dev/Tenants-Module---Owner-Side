using System.Text.Json.Serialization;

namespace Maydon.Host.Endpoints.Identity.TestsBox;

public sealed record MyIdTokenResponse(
    [property: JsonPropertyName("access_token")] string AccessToken,
    [property: JsonPropertyName("expires_in")] int ExpiresIn,
    [property: JsonPropertyName("token_type")] string TokenType);