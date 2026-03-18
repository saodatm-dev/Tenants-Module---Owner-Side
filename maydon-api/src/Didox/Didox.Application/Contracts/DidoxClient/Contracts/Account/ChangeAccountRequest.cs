using System.Text.Json.Serialization;

namespace Didox.Application.Contracts.DidoxClient.Contracts.Account;

public class ChangeAccountRequest
{
    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("mobile")]
    public string Mobile { get; set; } = default!;

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("email")]
    public string Email { get; set; } = default!;

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("notifications")]
    public int Notifications { get; set; }
}

