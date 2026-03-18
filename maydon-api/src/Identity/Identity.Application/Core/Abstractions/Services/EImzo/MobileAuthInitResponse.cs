using System.Text.Json.Serialization;

namespace Identity.Application.Core.Abstractions.Services.EImzo;

public sealed record MobileAuthInitResponse(
	[property: JsonPropertyName("status")] int Status,
	[property: JsonPropertyName("siteId")] string SiteId,
	[property: JsonPropertyName("documentId")] string DocumentId,
	[property: JsonPropertyName("challange")] string Challenge,
	[property: JsonPropertyName("message")] string Message);
