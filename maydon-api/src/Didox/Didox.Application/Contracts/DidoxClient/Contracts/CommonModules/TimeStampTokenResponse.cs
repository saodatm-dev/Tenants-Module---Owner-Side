using System.Text.Json.Serialization;

namespace Didox.Application.Contracts.DidoxClient.Contracts.CommonModules;

/// <summary>
/// Response containing the result of generating a timestamp token (Time Stamp Token).
/// Returned after a successful request to the timestamp service.
/// </summary>
public class TimeStampTokenResponse
{
    /// <summary>
    /// Time Stamp Token, encoded in Base64 format.
    /// Used to confirm the signing time and ensure the integrity of the signed data.
    /// </summary>
    [JsonPropertyName("timeStampTokenB64")]
    public string TimeStampTokenB64 { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether the timestamp generation request was successful.
    /// <c>true</c> means the timestamp was successfully generated.
    /// </summary>
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    /// <summary>
    /// Indicates whether the timestamp token is embedded inside a PKCS#7 structure.
    /// <c>true</c> means the token is included as an attached PKCS#7 signature.
    /// </summary>
    [JsonPropertyName("isAttachedPkcs7")]
    public bool IsAttachedPkcs7 { get; set; }
}

