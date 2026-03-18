using System.ComponentModel.DataAnnotations;

namespace Core.Infrastructure.Configuration;

/// <summary>
/// Configuration options for Redis Streams cleanup worker.
/// Controls retention policies for messages, consumer groups, pending entries, and dead letter queues.
/// </summary>
public sealed class RedisStreamsCleanupOptions
{
    public const string SectionName = "RedisStreamsCleanup";

    /// <summary>
    /// Whether the cleanup worker is enabled.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Interval between cleanup runs (in minutes).
    /// Default: 60 minutes (1 hour)
    /// </summary>
    [Range(5, 1440)]
    public int IntervalMinutes { get; set; } = 60;

    /// <summary>
    /// Maximum age for acknowledged messages (in hours).
    /// Messages older than this and acknowledged will be deleted.
    /// Default: 24 hours
    /// </summary>
    [Range(1, 720)]
    public int AcknowledgedMessageRetentionHours { get; set; } = 24;

    /// <summary>
    /// Maximum age for unacknowledged messages in streams (in days).
    /// Unacknowledged messages older than this will be logged for investigation.
    /// Default: 7 days
    /// </summary>
    [Range(1, 90)]
    public int MessageRetentionDays { get; set; } = 7;

    /// <summary>
    /// Number of days of inactivity before a consumer group is considered stale.
    /// Default: 30 days
    /// </summary>
    [Range(7, 365)]
    public int InactiveConsumerGroupDays { get; set; } = 30;

    /// <summary>
    /// Timeout for pending messages (in hours).
    /// Pending messages stuck longer than this will be reclaimed or acknowledged.
    /// Default: 6 hours
    /// </summary>
    [Range(1, 72)]
    public int PendingMessageTimeoutHours { get; set; } = 6;

    /// <summary>
    /// Maximum age for Dead Letter Queue messages (in days).
    /// DLQ messages older than this will be deleted.
    /// Default: 30 days
    /// </summary>
    [Range(1, 365)]
    public int DlqRetentionDays { get; set; } = 30;

    /// <summary>
    /// Maximum number of messages per stream before triggering aggressive cleanup.
    /// Default: 100,000
    /// </summary>
    [Range(10_000, 1_000_000)]
    public int MaxMessagesPerStream { get; set; } = 100_000;

    /// <summary>
    /// Maximum number of messages in DLQ before sending alert.
    /// Default: 1000
    /// </summary>
    [Range(100, 100_000)]
    public int DlqAlertThreshold { get; set; } = 1000;

    /// <summary>
    /// Whether to cleanup idempotency keys during the cleanup cycle.
    /// Default: true
    /// </summary>
    public bool CleanupIdempotencyKeys { get; set; } = true;

    /// <summary>
    /// Whether to automatically trim streams to MaxMessagesPerStream.
    /// Default: true
    /// </summary>
    public bool EnableAutomaticTrimming { get; set; } = true;
}
