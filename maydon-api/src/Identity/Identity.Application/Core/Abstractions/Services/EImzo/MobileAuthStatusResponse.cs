using System.Text.Json.Serialization;

namespace Identity.Application.Core.Abstractions.Services.EImzo;

public sealed record MobileAuthStatusResponse(
	[property: JsonPropertyName("status")] int Status,
	[property: JsonPropertyName("message")] string Message);
