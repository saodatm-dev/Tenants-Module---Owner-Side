using System.Text.Json;
using Core.Application.Abstractions.Messaging;
using Core.Infrastructure.Configuration;
using Core.Infrastructure.Monitoring;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Core.Infrastructure.Messaging;

/// <summary>
/// Background worker that consumes integration events from Redis Streams.
/// Uses consumer groups for competing consumers pattern across multiple instances.
/// Discovers all registered IIntegrationEventHandler implementations and starts
/// consuming from the corresponding streams.
/// </summary>
public sealed class IntegrationEventConsumerWorker(
    IConnectionMultiplexer redis,
    IServiceScopeFactory serviceScopeFactory,
    IOptions<IntegrationEventOptions> options,
    IntegrationEventMetrics metrics,
    ILogger<IntegrationEventConsumerWorker> logger) : BackgroundService
{
    private readonly IDatabase _database = redis.GetDatabase();
    private readonly IntegrationEventOptions _options = options.Value;
    private readonly string _consumerId = $"{Environment.MachineName}-{Guid.NewGuid().ToString("N")[..8]}";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Allow the rest of startup to complete before we start consuming
        await Task.Yield();

        if (!_options.EnableConsumer)
        {
            logger.LogInformation("IntegrationEventConsumerWorker is disabled via configuration (EnableConsumer=false). Skipping.");
            return;
        }

        logger.LogInformation(
            "IntegrationEventConsumerWorker starting with consumer group {GroupName}, consumer {ConsumerId}",
            _options.ConsumerGroupName, _consumerId);

        // Discover all event types that have registered handlers
        var eventTypes = DiscoverEventTypes();

        if (eventTypes.Count == 0)
        {
            logger.LogWarning("No integration event handlers found. Consumer worker will not start stream consumers.");
            return;
        }

        logger.LogInformation("Discovered {Count} event types with handlers: {EventTypes}",
            eventTypes.Count, string.Join(", ", eventTypes.Select(t => t.Name)));

        // Ensure consumer groups exist for all streams
        foreach (var eventType in eventTypes)
        {
            var streamKey = GetStreamKey(eventType.Name);
            await EnsureConsumerGroupAsync(streamKey);
        }

        // Start a consumer task per event type
        var consumerTasks = eventTypes
            .Select(eventType => ConsumeStreamAsync(eventType, stoppingToken))
            .ToList();

        await Task.WhenAll(consumerTasks);

        logger.LogInformation("IntegrationEventConsumerWorker stopped");
    }

    /// <summary>
    /// Discovers all integration event types that have at least one handler registered.
    /// </summary>
    private List<Type> DiscoverEventTypes()
    {
        using var scope = serviceScopeFactory.CreateScope();
        var handlerInterfaceType = typeof(IIntegrationEventHandler<>);

        // Find all registered services that implement IIntegrationEventHandler<T>
        var eventTypes = scope.ServiceProvider
            .GetServices<IServiceProviderIsService>()
            .FirstOrDefault();

        // Alternative: scan registered service descriptors
        // We check the service collection indirectly by using known event types
        var knownEventTypes = new List<Type>();

        // Try to resolve handlers for known event types
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            try
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (!type.IsClass || type.IsAbstract) continue;

                    foreach (var iface in type.GetInterfaces())
                    {
                        if (iface.IsGenericType && iface.GetGenericTypeDefinition() == handlerInterfaceType)
                        {
                            var eventType = iface.GetGenericArguments()[0];
                            if (!knownEventTypes.Contains(eventType))
                            {
                                // Verify the handler is actually registered in DI
                                var resolvedHandlerType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
                                var handler = scope.ServiceProvider.GetService(resolvedHandlerType);
                                if (handler is not null)
                                {
                                    knownEventTypes.Add(eventType);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogDebug(ex, "Could not scan assembly {Assembly} for event handlers", assembly.FullName);
            }
        }

        return knownEventTypes;
    }

    private async Task EnsureConsumerGroupAsync(string streamKey)
    {
        try
        {
            await _database.StreamCreateConsumerGroupAsync(streamKey, _options.ConsumerGroupName, "0-0", createStream: true);
            logger.LogInformation("Created consumer group {GroupName} for stream {StreamKey}",
                _options.ConsumerGroupName, streamKey);
        }
        catch (RedisServerException ex) when (ex.Message.Contains("BUSYGROUP"))
        {
            logger.LogDebug("Consumer group {GroupName} already exists for stream {StreamKey}",
                _options.ConsumerGroupName, streamKey);
        }
    }

    private async Task ConsumeStreamAsync(Type eventType, CancellationToken cancellationToken)
    {
        var streamKey = GetStreamKey(eventType.Name);

        logger.LogInformation("Starting consumer for {EventType} on stream {StreamKey}",
            eventType.Name, streamKey);

        // Process any pending messages first (from crashed consumers)
        await ProcessPendingMessagesAsync(eventType, streamKey, cancellationToken);

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var entries = await _database.StreamReadGroupAsync(
                    streamKey,
                    _options.ConsumerGroupName,
                    _consumerId,
                    ">",
                    count: _options.PrefetchCount);

                if (entries.Length == 0)
                {
                    await Task.Delay(_options.BlockMilliseconds, cancellationToken);
                    continue;
                }

                metrics.IncrementPendingMessages(eventType.Name, entries.Length);

                foreach (var entry in entries)
                {
                    try
                    {
                        await ProcessMessageAsync(eventType, streamKey, entry, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error processing message {MessageId} from {StreamKey}",
                            entry.Id, streamKey);

                        if (_options.EnableDeadLetterQueue)
                        {
                            await MoveToDeadLetterQueueAsync(streamKey, entry, ex);
                        }
                    }
                    finally
                    {
                        metrics.DecrementPendingMessages(eventType.Name);
                    }
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in consumer loop for {StreamKey}", streamKey);
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            }
        }
    }

    private async Task ProcessPendingMessagesAsync(Type eventType, string streamKey, CancellationToken cancellationToken)
    {
        try
        {
            var pendingMessages = await _database.StreamReadGroupAsync(
                streamKey, _options.ConsumerGroupName, _consumerId, "0-0", count: 100);

            if (pendingMessages.Length > 0)
            {
                logger.LogInformation("Processing {Count} pending messages from {StreamKey}",
                    pendingMessages.Length, streamKey);

                foreach (var entry in pendingMessages)
                {
                    try
                    {
                        await ProcessMessageAsync(eventType, streamKey, entry, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning(ex, "Failed to process pending message {MessageId}", entry.Id);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Error processing pending messages for {StreamKey}", streamKey);
        }
    }

    private async Task ProcessMessageAsync(Type eventType, string streamKey, StreamEntry entry, CancellationToken cancellationToken)
    {
        var payload = entry.Values.FirstOrDefault(v => v.Name == "payload").Value;
        if (payload.IsNullOrEmpty)
        {
            logger.LogWarning("Empty payload in message {MessageId} from {StreamKey}", entry.Id, streamKey);
            await _database.StreamAcknowledgeAsync(streamKey, _options.ConsumerGroupName, entry.Id);
            return;
        }

        var @event = JsonSerializer.Deserialize((string)payload!, eventType, JsonOptions);
        if (@event is null)
        {
            logger.LogWarning("Failed to deserialize message {MessageId} from {StreamKey} as {EventType}",
                entry.Id, streamKey, eventType.Name);
            await _database.StreamAcknowledgeAsync(streamKey, _options.ConsumerGroupName, entry.Id);
            return;
        }

        // Dispatch through the pipeline using a scoped service provider
        await using var scope = serviceScopeFactory.CreateAsyncScope();
        var pipeline = scope.ServiceProvider.GetRequiredService<IIntegrationEventHandlerPipeline>();

        // Use reflection to call HandleAsync<TEvent> with the correct type parameter
        var handleMethod = typeof(IIntegrationEventHandlerPipeline)
            .GetMethod(nameof(IIntegrationEventHandlerPipeline.HandleAsync))!
            .MakeGenericMethod(eventType);

        await (Task)handleMethod.Invoke(pipeline, [@event, cancellationToken])!;

        await _database.StreamAcknowledgeAsync(streamKey, _options.ConsumerGroupName, entry.Id);
    }

    private async Task MoveToDeadLetterQueueAsync(string streamKey, StreamEntry entry, Exception exception)
    {
        try
        {
            var dlqKey = $"{streamKey}:dlq";
            var dlqEntries = entry.Values.ToList();
            dlqEntries.Add(new NameValueEntry("dlq_timestamp", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()));
            dlqEntries.Add(new NameValueEntry("dlq_reason", exception.Message));
            dlqEntries.Add(new NameValueEntry("dlq_original_stream", streamKey));

            await _database.StreamAddAsync(dlqKey, dlqEntries.ToArray());
            await _database.StreamAcknowledgeAsync(streamKey, _options.ConsumerGroupName, entry.Id);

            var eventType = entry.Values.FirstOrDefault(v => v.Name == "eventType").Value;
            metrics.RecordEventSentToDlq(eventType!, exception.GetType().Name);

            logger.LogWarning("Moved message {MessageId} to DLQ {DlqKey}: {Error}",
                entry.Id, dlqKey, exception.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to move message {MessageId} to DLQ", entry.Id);
        }
    }

    private static string GetStreamKey(string eventType) => $"integration-events-stream:{eventType}";
}
