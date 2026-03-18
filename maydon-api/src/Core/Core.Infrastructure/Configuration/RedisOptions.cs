using System.ComponentModel.DataAnnotations;

namespace Core.Infrastructure.Configuration;

/// <summary>
/// Configuration options for Redis connection.
/// </summary>
public sealed class RedisOptions
{
    /// <summary>
    /// Configuration section name.
    /// </summary>
    public const string SectionName = "Redis";

    /// <summary>
    /// Channel prefix for Redis pub/sub channels.
    /// Default: "maydon:"
    /// </summary>
    [Required(ErrorMessage = "Redis channel prefix is required")]
    public string ChannelPrefix { get; set; } = "maydon:";

    /// <summary>
    /// Maximum number of retry attempts for connection failures.
    /// Default: 3
    /// </summary>
    [Range(1, 10, ErrorMessage = "MaxRetryAttempts must be between 1 and 10")]
    public int MaxRetryAttempts { get; set; } = 3;

    /// <summary>
    /// Connection timeout in milliseconds.
    /// Default: 5000ms (5 seconds)
    /// </summary>
    [Range(1000, 30000, ErrorMessage = "ConnectTimeoutMs must be between 1000 and 30000")]
    public int ConnectTimeoutMs { get; set; } = 5000;

    /// <summary>
    /// Synchronous operation timeout in milliseconds.
    /// Default: 5000ms (5 seconds)
    /// </summary>
    [Range(1000, 30000, ErrorMessage = "SyncTimeoutMs must be between 1000 and 30000")]
    public int SyncTimeoutMs { get; set; } = 5000;
}
