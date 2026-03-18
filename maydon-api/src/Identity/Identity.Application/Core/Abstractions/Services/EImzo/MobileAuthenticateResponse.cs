using System.Text.Json.Serialization;

namespace Identity.Application.Core.Abstractions.Services.EImzo;

public sealed record MobileAuthenticateResponse(
	[property: JsonPropertyName("status")] int Status,
	[property: JsonPropertyName("subjectCertificateInfo")] SubjectCertificateInfo SubjectCertificateInfo,
	[property: JsonPropertyName("message")] string Message);
