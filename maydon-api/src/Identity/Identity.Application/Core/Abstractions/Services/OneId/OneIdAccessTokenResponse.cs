using System.Text.Json.Serialization;

namespace Identity.Application.Core.Abstractions.Services.OneId;

public sealed record OneIdAccessTokenResponse(
	[property: JsonPropertyName("scope")] string Scope,
	[property: JsonPropertyName("expires_in")] long ExpiredTime,
	[property: JsonPropertyName("token_type")] string TokenType,
	[property: JsonPropertyName("refresh_token")] Guid RefreshToken,
	[property: JsonPropertyName("access_token")] Guid AccessToken);
