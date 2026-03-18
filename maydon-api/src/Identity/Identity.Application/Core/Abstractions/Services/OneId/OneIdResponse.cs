using System.Text.Json.Serialization;
using Core.Application.Helpers;

namespace Identity.Application.Core.Abstractions.Services.OneId;

public sealed record OneIdResponse(
	[property: JsonPropertyName("valid")] string IsValid,
	[property: JsonPropertyName("pin")] string Pinfl,
	[property: JsonPropertyName("user_id")] string Login,
	[property: JsonPropertyName("full_name")] string FullName,
	[property: JsonPropertyName("pport_no")] string PasportNumber,
	[property: JsonPropertyName("birth_date")]
	[property: JsonConverter(typeof(JsonDateOnlyConverter))] DateOnly? BirthDate,
	[property: JsonPropertyName("sur_name")] string LastName,
	[property: JsonPropertyName("first_name")] string FirstName,
	[property: JsonPropertyName("mid_name")] string? MiddleName,
	[property: JsonPropertyName("user_type")] char UserType,          // I-Jismoniy shaxs, L-Yuridik shaxs
	[property: JsonPropertyName("sess_id")] Guid SessionId,
	[property: JsonPropertyName("ret_cd")] string? RetCd,
	[property: JsonPropertyName("auth_method")] string AuthMethod,
	[property: JsonPropertyName("pkcs_legal_tin")] string? CompanyPKCSTin,   //izoh: agar auth_methodning qiymati LEPKCSMETHOD boʻlsa ma’lumot taqdim etiladi, aks holda ushbu satr berilmaydi.
	[property: JsonPropertyName("legal_info")] CompanyInfo[]? CompanyInfo);

public sealed record CompanyInfo(
	[property: JsonPropertyName("is_basic")] bool IsBasic,
	[property: JsonPropertyName("tin")] string Tin,
	[property: JsonPropertyName("acron_UZ")] string Name,
	[property: JsonPropertyName("le_tin")] string CompanyTin,
	[property: JsonPropertyName("le_name")] string CompanyName);
