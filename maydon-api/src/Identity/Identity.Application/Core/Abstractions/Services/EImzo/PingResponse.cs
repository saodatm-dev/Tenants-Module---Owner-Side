using System.Text.Json.Serialization;

namespace Identity.Application.Core.Abstractions.Services.EImzo;

public sealed record PingResponse(
	[property: JsonPropertyName("serverDateTime")] string ServerDateTime,
	[property: JsonPropertyName("yourIP")] string YourIp,
	[property: JsonPropertyName("vpnKeyInfo")] VPNKeyInfo VpnKeyInfo);

public sealed record VPNKeyInfo(
	[property: JsonPropertyName("serialNumber")] string SerialNumber,
	[property: JsonPropertyName("X500Name")] string X500Name,
	[property: JsonPropertyName("validFrom")] string ValidFrom,
	[property: JsonPropertyName("validTo")] string ValidTo);
