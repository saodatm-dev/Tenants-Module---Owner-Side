using Core.Application.Abstractions.Jobs;
using Core.Application.Abstractions.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Core.Infrastructure.Versioning;

/// <summary>
/// Handles EntityVersionChangedEvent from Redis Streams and executes versioning workflow.
/// Replaces the previous Redis Pub/Sub RedisMessageConsumer for better reliability.
/// This handler:
/// 1. Persists version to MongoDB (cold storage - full history)
/// 2. Sends SignalR notifications to connected clients
/// Uses idempotency to prevent duplicate processing in multi-instance scenarios.
/// </summary>
public sealed class EntityVersionChangedHandler(
    IServiceScopeFactory serviceScopeFactory,
    IConnectionMultiplexer redis,
    IIdempotencyStore idempotencyStore,
    ILogger<EntityVersionChangedHandler> logger)
    : IdempotentIntegrationEventHandlerBase<EntityVersionChangedEvent>(idempotencyStore, logger)
{
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
    private readonly IConnectionMultiplexer _redis = redis ?? throw new ArgumentNullException(nameof(redis));

    protected override async Task HandleIdempotentAsync(EntityVersionChangedEvent @event, CancellationToken cancellationToken)
    {
        Logger.LogInformation("Processing EntityVersionChangedEvent for {EntityType} {EntityId} v{Version} ({ChangeType})", @event.EntityType, @event.EntityId, @event.VersionNumber, @event.ChangeType);

        var lockKey = $"version-lock:{@event.EntityId}:{@event.VersionNumber}";
        var lockExpiry = TimeSpan.FromSeconds(5);

        var database = _redis.GetDatabase();
        var lockAcquired = await database.StringSetAsync(lockKey, "locked", lockExpiry, When.NotExists);

        if (!lockAcquired)
        {
            Logger.LogDebug("Lock not acquired for {EntityId} v{Version}, another instance is processing this event", @event.EntityId, @event.VersionNumber);
            return;
        }

        try
        {
            await using var scope = _serviceScopeFactory.CreateAsyncScope();

            var workflowProcessor = scope.ServiceProvider.GetRequiredService<IVersioningWorkflowProcessor>();
            await workflowProcessor.ExecuteAsync(@event, cancellationToken);

            Logger.LogInformation("Completed versioning workflow for {EntityType} {EntityId} v{Version}", @event.EntityType, @event.EntityId, @event.VersionNumber);
        }
        finally
        {
            await database.KeyDeleteAsync(lockKey);
        }
    }
}
