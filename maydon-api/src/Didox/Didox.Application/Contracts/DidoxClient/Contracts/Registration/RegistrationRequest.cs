using System.Text.Json.Serialization;

namespace Didox.Application.Contracts.DidoxClient.Contracts.Registration;

/// <summary>
/// User registration request model.
/// Used to send registration data in JSON format.
/// </summary>
public record RegistrationRequest
{
    /// <summary>
    /// User's email address.
    /// Used as the primary identifier for login and communication.
    /// </summary>
    [JsonPropertyName("email")]
    public required string Email { get; set; }

    /// <summary>
    /// User's mobile phone number.
    /// May be used for identity verification or two-factor authentication.
    /// </summary>
    [JsonPropertyName("mobile")]
    public required string Mobile { get; set; }

    /// <summary>
    /// User's password.
    /// Must meet the system's security requirements.
    /// </summary>
    [JsonPropertyName("password")]
    public required string Password { get; set; }

    /// <summary>
    /// Flag indicating user's consent to terms of use and privacy policy.
    /// Value <c>true</c> means the user has accepted the terms.
    /// </summary>
    [JsonPropertyName("accept")]
    public bool Accept { get; set; }

    /// <summary>
    /// Digital signature of the request data.
    /// Used to verify the integrity and authenticity of the transmitted information.
    /// </summary>
    [JsonPropertyName("signature")]
    public required string Signature { get; set; }
}

