using System.Text.Json.Serialization;

namespace Didox.Application.Contracts.DidoxClient.Contracts.Account;

/// <summary>
/// Represent Didox Account 
/// </summary>
public class AccountResponse
{
    /// <summary>
    /// Phone number of user in Didox
    /// </summary>
    [JsonPropertyName("mobile")]
    public string Mobile { get; set; } = default!;

    /// <summary>
    /// Email of user in Didox system
    /// </summary>
    [JsonPropertyName("email")]
    public string Email { get; set; } = default!;

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("notifications")]
    public int Notifications { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("messengers")]
    public IReadOnlyList<string>? Messengers { get; set; }
}

