using Core.Infrastructure.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Core.Infrastructure.Jobs;

/// <summary>
/// Background worker that performs periodic cleanup of Redis Streams to prevent memory bloat.
/// Implements multiple cleanup strategies:
/// 1. Time-based cleanup: Delete old acknowledged messages
/// 2. Consumer group cleanup: Remove inactive consumer groups
/// 3. PEL cleanup: Reclaim or acknowledge stale pending messages
/// 4. DLQ management: Archive and cleanup Dead Letter Queue
/// 5. Size-based cleanup: Trigger aggressive cleanup if stream too large
/// </summary>
public sealed class RedisStreamsCleanupWorker(
    IConnectionMultiplexer redis,
    IOptionsMonitor<RedisStreamsCleanupOptions> optionsMonitor,
    ILogger<RedisStreamsCleanupWorker> logger)
    : BackgroundService
{
    private readonly IConnectionMultiplexer _redis = redis ?? throw new ArgumentNullException(nameof(redis));
    private readonly IOptionsMonitor<RedisStreamsCleanupOptions> _optionsMonitor = optionsMonitor ?? throw new ArgumentNullException(nameof(optionsMonitor));
    private readonly ILogger<RedisStreamsCleanupWorker> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private const string StreamKeyPrefix = "integration-events-stream";
    private const string DlqSuffix = "dlq";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var options = _optionsMonitor.CurrentValue;
        
        if (!options.Enabled)
        {
            _logger.LogInformation("RedisStreamsCleanupWorker is disabled. Exiting.");
            return;
        }

        var interval = TimeSpan.FromMinutes(options.IntervalMinutes);
        _logger.LogInformation(
            "RedisStreamsCleanupWorker started. Cleanup interval: {Interval}, Acknowledged retention: {AckRetention}h, Message retention: {MsgRetention}d",
            interval, options.AcknowledgedMessageRetentionHours, options.MessageRetentionDays);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var currentOptions = _optionsMonitor.CurrentValue;
                var currentInterval = TimeSpan.FromMinutes(currentOptions.IntervalMinutes);
                
                await Task.Delay(currentInterval, stoppingToken);
                await RunCleanupCycleAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during Redis Streams cleanup cycle");
            }
        }

        _logger.LogInformation("RedisStreamsCleanupWorker stopped");
    }

    private async Task RunCleanupCycleAsync(CancellationToken cancellationToken)
    {
        var options = _optionsMonitor.CurrentValue;
        
        _logger.LogInformation("Starting Redis Streams cleanup cycle...");
        var database = _redis.GetDatabase();
        var server = _redis.GetServer(_redis.GetEndPoints().First());

        var streamKeys = server.Keys(pattern: $"{StreamKeyPrefix}:*")
            .Where(k => !k.ToString().EndsWith($":{DlqSuffix}"))
            .ToList();

        _logger.LogInformation("Found {Count} streams to process", streamKeys.Count);

        int totalDeleted = 0;
        int totalReclaimed = 0;
        int totalConsumerGroupsRemoved = 0;

        foreach (var streamKey in streamKeys)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                var deleted = await CleanupAcknowledgedMessagesAsync(database, streamKey, options, cancellationToken);
                totalDeleted += deleted;

                var reclaimed = await CleanupPendingEntriesAsync(database, streamKey, options, cancellationToken);
                totalReclaimed += reclaimed;

                if (options.EnableAutomaticTrimming)
                {
                    await TrimStreamToMaxLengthAsync(database, streamKey, options, cancellationToken);
                }

                var groupsRemoved = await CleanupInactiveConsumerGroupsAsync(database, streamKey, options, cancellationToken);
                totalConsumerGroupsRemoved += groupsRemoved;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cleaning up stream {StreamKey}", streamKey);
            }
        }

        await CleanupDeadLetterQueuesAsync(database, server, options, cancellationToken);

        if (options.CleanupIdempotencyKeys)
        {
            await CleanupIdempotencyKeysAsync(database, server, cancellationToken);
        }

        if (options.EnableAutomaticTrimming)
        {
            await TrimAllStreamsAsync(database, server, options, cancellationToken);
        }

        _logger.LogInformation(
            "Redis Streams cleanup cycle completed. Deleted: {Deleted}, Reclaimed: {Reclaimed}, Consumer Groups Removed: {GroupsRemoved}",
            totalDeleted, totalReclaimed, totalConsumerGroupsRemoved);
    }

    private async Task<int> CleanupAcknowledgedMessagesAsync(
        IDatabase database, RedisKey streamKey, RedisStreamsCleanupOptions options, CancellationToken cancellationToken)
    {
        try
        {
            var cutoffTime = DateTime.UtcNow.AddHours(-options.AcknowledgedMessageRetentionHours);
            var cutoffMillis = new DateTimeOffset(cutoffTime).ToUnixTimeMilliseconds();

            var streamInfo = await database.StreamInfoAsync(streamKey);
            if (streamInfo.Length == 0) return 0;

            var groups = await database.StreamGroupInfoAsync(streamKey);
            if (groups == null || groups.Length == 0)
            {
                var ageBasedMessages = await database.StreamRangeAsync(streamKey, minId: "-", maxId: cutoffMillis.ToString(), count: 1000);
                if (ageBasedMessages.Length > 0)
                {
                    var ageBasedDeletes = ageBasedMessages.Select(m => m.Id).ToArray();
                    var deleted = await database.StreamDeleteAsync(streamKey, ageBasedDeletes);
                    _logger.LogInformation("Deleted {Count} old messages from stream {Stream} with no consumer groups", deleted, streamKey);
                    return (int)deleted;
                }
                return 0;
            }

            var oldMessages = await database.StreamRangeAsync(streamKey, minId: "-", maxId: cutoffMillis.ToString(), count: 1000);
            if (oldMessages.Length == 0) return 0;

            var allPendingIds = new HashSet<RedisValue>();
            foreach (var group in groups)
            {
                try
                {
                    var pendingInfo = await database.StreamPendingAsync(streamKey, group.Name);
                    if (pendingInfo.PendingMessageCount > 0)
                    {
                        var pendingMessages = await database.StreamPendingMessagesAsync(
                            streamKey, group.Name, count: oldMessages.Length,
                            consumerName: RedisValue.Null, minId: oldMessages[0].Id, maxId: oldMessages[^1].Id);
                        foreach (var pending in pendingMessages)
                        {
                            allPendingIds.Add(pending.MessageId);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to get pending messages for group {Group}", group.Name);
                }
            }

            var messagesToDelete = oldMessages.Where(m => !allPendingIds.Contains(m.Id)).Select(m => m.Id).ToList();
            if (messagesToDelete.Count > 0)
            {
                var deleted = await database.StreamDeleteAsync(streamKey, messagesToDelete.ToArray());
                return (int)deleted;
            }
            return 0;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to cleanup acknowledged messages from {Stream}", streamKey);
            return 0;
        }
    }

    private async Task<int> CleanupPendingEntriesAsync(
        IDatabase database, RedisKey streamKey, RedisStreamsCleanupOptions options, CancellationToken cancellationToken)
    {
        try
        {
            var groups = await database.StreamGroupInfoAsync(streamKey);
            if (groups is null || groups.Length == 0) return 0;

            int reclaimed = 0;
            foreach (var group in groups)
            {
                try
                {
                    var pendingMessages = await database.StreamPendingMessagesAsync(
                        streamKey, group.Name, count: 100, consumerName: RedisValue.Null);
                    if (pendingMessages.Length == 0) continue;

                    foreach (var pendingMessage in pendingMessages)
                    {
                        if (pendingMessage.IdleTimeInMilliseconds > TimeSpan.FromHours(options.PendingMessageTimeoutHours).TotalMilliseconds)
                        {
                            try
                            {
                                var dlqKey = $"{streamKey}:{DlqSuffix}";
                                var messages = await database.StreamRangeAsync(streamKey, pendingMessage.MessageId, pendingMessage.MessageId, count: 1);
                                if (messages.Length > 0)
                                {
                                    var dlqEntries = messages[0].Values.ToList();
                                    dlqEntries.Add(new NameValueEntry("dlq_timestamp", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()));
                                    dlqEntries.Add(new NameValueEntry("dlq_reason", "pending_timeout"));
                                    dlqEntries.Add(new NameValueEntry("dlq_original_id", pendingMessage.MessageId));
                                    await database.StreamAddAsync(dlqKey, dlqEntries.ToArray());
                                    await database.StreamAcknowledgeAsync(streamKey, group.Name, pendingMessage.MessageId);
                                    reclaimed++;
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogWarning(ex, "Failed to move message {MessageId} to DLQ", pendingMessage.MessageId);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to process pending messages for group {Group}", group.Name);
                }
            }
            return reclaimed;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to cleanup pending entries from {Stream}", streamKey);
            return 0;
        }
    }

    private async Task TrimStreamToMaxLengthAsync(
        IDatabase database, RedisKey streamKey, RedisStreamsCleanupOptions options, CancellationToken cancellationToken)
    {
        try
        {
            var streamInfo = await database.StreamInfoAsync(streamKey);
            if (streamInfo.Length > options.MaxMessagesPerStream)
            {
                await database.StreamTrimAsync(streamKey, maxLength: options.MaxMessagesPerStream, useApproximateMaxLength: true);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to trim stream {Stream}", streamKey);
        }
    }

    private async Task<int> CleanupInactiveConsumerGroupsAsync(
        IDatabase database, RedisKey streamKey, RedisStreamsCleanupOptions options, CancellationToken cancellationToken)
    {
        try
        {
            var groups = await database.StreamGroupInfoAsync(streamKey);
            if (groups is null || groups.Length == 0) return 0;

            int removed = 0;
            foreach (var group in groups)
            {
                try
                {
                    var consumers = await database.StreamConsumerInfoAsync(streamKey, group.Name);
                    if (consumers is null || consumers.Length == 0)
                    {
                        var pendingInfo = await database.StreamPendingAsync(streamKey, group.Name);
                        if (pendingInfo.PendingMessageCount == 0)
                        {
                            var deleted = await database.StreamDeleteConsumerGroupAsync(streamKey, group.Name);
                            if (deleted) removed++;
                        }
                    }
                    else
                    {
                        foreach (var consumer in consumers)
                        {
                            if (consumer.IdleTimeInMilliseconds > TimeSpan.FromDays(options.InactiveConsumerGroupDays).TotalMilliseconds)
                            {
                                var consumerPending = await database.StreamPendingMessagesAsync(
                                    streamKey, group.Name, count: 1, consumerName: consumer.Name);
                                if (consumerPending.Length == 0)
                                {
                                    await database.StreamDeleteConsumerAsync(streamKey, group.Name, consumer.Name);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to process consumer group {Group}", group.Name);
                }
            }
            return removed;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to cleanup consumer groups from {Stream}", streamKey);
            return 0;
        }
    }

    private async Task CleanupDeadLetterQueuesAsync(
        IDatabase database, IServer server, RedisStreamsCleanupOptions options, CancellationToken cancellationToken)
    {
        try
        {
            var dlqKeys = server.Keys(pattern: $"{StreamKeyPrefix}:*:{DlqSuffix}").ToList();
            foreach (var dlqKey in dlqKeys)
            {
                try
                {
                    var cutoffTime = DateTime.UtcNow.AddDays(-options.DlqRetentionDays);
                    var cutoffMillis = new DateTimeOffset(cutoffTime).ToUnixTimeMilliseconds();
                    var oldMessages = await database.StreamRangeAsync(dlqKey, minId: "-", maxId: cutoffMillis.ToString(), count: 1000);
                    if (oldMessages.Length > 0)
                    {
                        var messagesToDelete = oldMessages.Select(m => m.Id).ToArray();
                        await database.StreamDeleteAsync(dlqKey, messagesToDelete);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing DLQ {DlqKey}", dlqKey);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during DLQ cleanup");
        }
    }

    private async Task CleanupIdempotencyKeysAsync(IDatabase database, IServer server, CancellationToken cancellationToken)
    {
        try
        {
            var idempotencyKeys = server.Keys(pattern: "processed-event:*").ToList();
            if (idempotencyKeys.Count == 0) return;

            foreach (var key in idempotencyKeys)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var ttl = await database.KeyTimeToLiveAsync(key);
                    if (ttl == null)
                    {
                        await database.KeyDeleteAsync(key);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to process idempotency key {Key}", key);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during idempotency keys cleanup");
        }
    }

    private async Task TrimAllStreamsAsync(
        IDatabase database, IServer server, RedisStreamsCleanupOptions options, CancellationToken cancellationToken)
    {
        try
        {
            var streamKeys = server.Keys(pattern: $"{StreamKeyPrefix}:*")
                .Where(k => !k.ToString().EndsWith($":{DlqSuffix}"))
                .ToList();

            foreach (var streamKey in streamKeys)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var streamInfo = await database.StreamInfoAsync(streamKey);
                    if (streamInfo.Length > options.MaxMessagesPerStream)
                    {
                        await database.StreamTrimAsync(streamKey, maxLength: options.MaxMessagesPerStream, useApproximateMaxLength: true);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to trim stream {Stream}", streamKey);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during final stream trimming pass");
        }
    }
}
