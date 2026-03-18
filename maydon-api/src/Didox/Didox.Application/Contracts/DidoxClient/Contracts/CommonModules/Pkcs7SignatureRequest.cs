using System.Text.Json.Serialization;

namespace Didox.Application.Contracts.DidoxClient.Contracts.CommonModules;

/// <summary>
/// Request model for submitting a document signature in PKCS#7 format.
/// Contains the signed data itself as well as its hexadecimal representation.
/// </summary>
public class Pkcs7SignatureRequest
{
    /// <summary>
    /// Document signature in PKCS#7 format,
    /// encoded in Base64.
    /// Used to verify the authenticity and integrity of the document.
    /// </summary>
    [JsonPropertyName("pkcs7")]
    public string Pkcs7 { get; set; } = default!;

    /// <summary>
    /// Hexadecimal (HEX) representation of the signature.
    /// Used to perform additional validation and cryptographic checks.
    /// </summary>
    [JsonPropertyName("signatureHex")]
    public string SignatureHex { get; set; } = default!;
}


