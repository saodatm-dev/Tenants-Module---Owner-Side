using System.Text.Json.Serialization;

namespace Identity.Application.Core.Abstractions.Services.EImzo;

public sealed record AuthResponse(
	[property: JsonPropertyName("subjectCertificateInfo")] SubjectCertificateInfo SubjectCertificateInfo,
	[property: JsonPropertyName("status")] int Status,
	[property: JsonPropertyName("message")] string Message);

public sealed record SubjectCertificateInfo(
	[property: JsonPropertyName("serialNumber")] string SerialNumber,
	[property: JsonPropertyName("X500Name")] string X500Name,
	[property: JsonPropertyName("subjectName")] SubjectNameResponse SubjectName,
	[property: JsonPropertyName("validFrom")] string ValidFrom,
	[property: JsonPropertyName("validTo")] string ValidTo);

public sealed record SubjectNameResponse(
	[property: JsonPropertyName("St")] string Region,
	[property: JsonPropertyName("UID")] string TIN,
	[property: JsonPropertyName("SURNAME")] string LastName,
	[property: JsonPropertyName("C")] string Country,
	[property: JsonPropertyName("T")] string Position,
	[property: JsonPropertyName("BusinessCategory")] string BusinessCategory,
	[property: JsonPropertyName("1.2.860.3.16.1.2")] string PINFL,
	[property: JsonPropertyName("CN")] string FullName,
	[property: JsonPropertyName("L")] string District,
	[property: JsonPropertyName("1.2.860.3.16.1.1")] string OrganizationTIN,
	[property: JsonPropertyName("Name")] string FirstName,
	[property: JsonPropertyName("O")] string OrganizationName);
