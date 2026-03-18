namespace Didox.Infrastructure.Client.Options;

/// <summary>
/// Configuration options for Didox API
/// </summary>
public class DidoxOptions
{
    /// <summary>
    /// Configuration section name in appsettings
    /// </summary>
    public const string SectionName = "Didox";
    
    /// <summary>
    /// Base URL for Didox API
    /// </summary>
    public string BaseUrl { get; set; } = null!;
    
    /// <summary>
    /// Partner token for Didox API authentication
    /// </summary>
    public string PartnerToken { get; set; } = null!;
}
