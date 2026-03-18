using System.ComponentModel.DataAnnotations;

namespace Core.Infrastructure.Configuration;

/// <summary>
/// Configuration options for integration event infrastructure.
/// Controls behavior of Redis Streams-based event processing.
/// </summary>
public sealed class IntegrationEventOptions
{
    public const string SectionName = "IntegrationEvents";

    /// <summary>
    /// Consumer group name for Redis Streams consumer groups.
    /// All instances of the same service should use the same consumer group
    /// to enable competing consumers pattern.
    /// Default: "maydon-consumers"
    /// </summary>
    [Required]
    public string ConsumerGroupName { get; set; } = "maydon-consumers";

    /// <summary>
    /// Maximum number of retry attempts before sending to DLQ.
    /// Default: 3
    /// </summary>
    [Range(0, 10)]
    public int MaxRetryAttempts { get; set; } = 3;

    /// <summary>
    /// Initial retry delay in seconds (exponential backoff will multiply this).
    /// Default: 2 seconds
    /// </summary>
    [Range(1, 60)]
    public int InitialRetryDelaySeconds { get; set; } = 2;

    /// <summary>
    /// Number of messages to prefetch per consumer poll.
    /// Higher values improve throughput but increase memory usage.
    /// Default: 10
    /// </summary>
    [Range(1, 1000)]
    public int PrefetchCount { get; set; } = 10;

    /// <summary>
    /// Time in milliseconds to block waiting for new messages when polling.
    /// Default: 5000ms (5 seconds)
    /// </summary>
    [Range(1000, 60000)]
    public int BlockMilliseconds { get; set; } = 5000;

    /// <summary>
    /// Maximum number of messages to keep in each stream (MAXLEN trimming).
    /// Prevents unbounded growth of streams.
    /// Default: 100,000
    /// </summary>
    [Range(1000, 10_000_000)]
    public int MaxStreamLength { get; set; } = 100_000;

    /// <summary>
    /// Retention period for processed event IDs in idempotency cache.
    /// Events older than this will be removed from the cache.
    /// Default: 1 hour
    /// </summary>
    public TimeSpan IdempotencyCacheRetention { get; set; } = TimeSpan.FromHours(1);

    /// <summary>
    /// Interval for cleaning up old processed event IDs.
    /// Default: 10 minutes
    /// </summary>
    public TimeSpan IdempotencyCacheCleanupInterval { get; set; } = TimeSpan.FromMinutes(10);

    /// <summary>
    /// Enable distributed tracing with OpenTelemetry.
    /// Default: true
    /// </summary>
    public bool EnableDistributedTracing { get; set; } = true;

    /// <summary>
    /// Enable metrics collection.
    /// Default: true
    /// </summary>
    public bool EnableMetrics { get; set; } = true;

    /// <summary>
    /// Enable automatic retry with exponential backoff.
    /// Default: true
    /// </summary>
    public bool EnableRetry { get; set; } = true;

    /// <summary>
    /// Enable Dead Letter Queue for failed messages.
    /// Default: true
    /// </summary>
    public bool EnableDeadLetterQueue { get; set; } = true;

    /// <summary>
    /// Enable idempotency checking to prevent duplicate processing.
    /// Default: true
    /// </summary>
    public bool EnableIdempotency { get; set; } = true;

    /// <summary>
    /// Enable the background consumer worker that reads from Redis Streams.
    /// Set to false for hosts that should only publish events, not consume them.
    /// Default: true
    /// </summary>
    public bool EnableConsumer { get; set; } = true;
}
