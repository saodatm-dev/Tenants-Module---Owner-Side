using System.Diagnostics;
using System.Text.Json;
using Core.Application.Abstractions.Messaging;
using Core.Infrastructure.Configuration;
using Core.Infrastructure.Monitoring;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Core.Infrastructure.Messaging;

/// <summary>
/// Redis Streams-based integration event publisher.
/// Publishes events to named streams with JSON serialization.
/// </summary>
public sealed class RedisStreamsEventPublisher(
    IConnectionMultiplexer redis,
    IOptions<IntegrationEventOptions> options,
    IntegrationEventMetrics metrics,
    ILogger<RedisStreamsEventPublisher> logger) : IIntegrationEventPublisher
{
    private readonly IDatabase _database = redis.GetDatabase();
    private readonly IntegrationEventOptions _options = options.Value;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) 
        where TEvent : class, IIntegrationEvent
    {
        var eventType = typeof(TEvent).Name;
        var streamKey = GetStreamKey(eventType);
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var payload = JsonSerializer.Serialize(@event, JsonOptions);
            
            var entries = new NameValueEntry[]
            {
                new("eventType", eventType),
                new("eventId", @event.EventId.ToString()),
                new("occurredOn", @event.OccurredOn.ToString("O")),
                new("payload", payload),
                new("clrType", typeof(TEvent).AssemblyQualifiedName ?? eventType)
            };

            var messageId = await _database.StreamAddAsync(
                streamKey, 
                entries, 
                maxLength: _options.MaxStreamLength,
                useApproximateMaxLength: true);

            stopwatch.Stop();
            metrics.RecordEventPublished(eventType, stopwatch.ElapsedMilliseconds);

            logger.LogDebug(
                "Published event {EventType} with ID {EventId} to stream {StreamKey} (messageId: {MessageId})",
                eventType, @event.EventId, streamKey, messageId);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            logger.LogError(ex, "Failed to publish event {EventType} with ID {EventId}", eventType, @event.EventId);
            throw;
        }
    }

    public async Task PublishBatchAsync<TEvent>(IEnumerable<TEvent> events, CancellationToken cancellationToken = default) 
        where TEvent : class, IIntegrationEvent
    {
        var eventList = events.ToList();
        if (eventList.Count == 0) return;

        var eventType = typeof(TEvent).Name;
        var streamKey = GetStreamKey(eventType);
        var stopwatch = Stopwatch.StartNew();
        var batch = _database.CreateBatch();

        try
        {
            var tasks = new List<Task<RedisValue>>();

            foreach (var @event in eventList)
            {
                var payload = JsonSerializer.Serialize(@event, JsonOptions);
                
                var entries = new NameValueEntry[]
                {
                    new("eventType", eventType),
                    new("eventId", @event.EventId.ToString()),
                    new("occurredOn", @event.OccurredOn.ToString("O")),
                    new("payload", payload),
                    new("clrType", typeof(TEvent).AssemblyQualifiedName ?? eventType)
                };

                tasks.Add(batch.StreamAddAsync(
                    streamKey, 
                    entries,
                    maxLength: _options.MaxStreamLength,
                    useApproximateMaxLength: true));
            }

            batch.Execute();
            await Task.WhenAll(tasks);

            stopwatch.Stop();
            metrics.RecordBatchPublished(eventType, eventList.Count, stopwatch.ElapsedMilliseconds);

            logger.LogInformation(
                "Published batch of {Count} {EventType} events to stream {StreamKey} in {ElapsedMs}ms",
                eventList.Count, eventType, streamKey, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            logger.LogError(ex, "Failed to publish batch of {Count} {EventType} events", eventList.Count, eventType);
            throw;
        }
    }

    private static string GetStreamKey(string eventType) => $"integration-events-stream:{eventType}";
}
