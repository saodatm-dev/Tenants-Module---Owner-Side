using Core.Application.Abstractions.Messaging;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Core.Infrastructure.Messaging;

/// <summary>
/// Redis-based idempotency store for integration events.
/// Uses Redis SET with TTL to track processed event IDs.
/// </summary>
public sealed class RedisIdempotencyStore(
    IConnectionMultiplexer redis,
    ILogger<RedisIdempotencyStore> logger) : IIdempotencyStore
{
    private readonly IDatabase _database = redis.GetDatabase();

    public async Task<bool> WasProcessedAsync(Guid eventId)
    {
        var key = GetKey(eventId);
        var exists = await _database.KeyExistsAsync(key);
        
        if (exists)
        {
            logger.LogDebug("Event {EventId} already processed (idempotency check)", eventId);
        }
        
        return exists;
    }

    public async Task MarkAsProcessedAsync(Guid eventId, TimeSpan expiry)
    {
        var key = GetKey(eventId);
        await _database.StringSetAsync(key, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), expiry);
        
        logger.LogDebug("Marked event {EventId} as processed (TTL: {Ttl})", eventId, expiry);
    }

    private static string GetKey(Guid eventId) => $"processed-event:{eventId}";
}
