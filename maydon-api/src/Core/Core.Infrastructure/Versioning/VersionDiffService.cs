using System.Text.Json;
using System.Text.Json.JsonDiffPatch;
using System.Text.Json.JsonDiffPatch.Diffs.Formatters;
using System.Text.Json.Nodes;
using Core.Application.Abstractions.Repositories;
using Core.Application.Abstractions.Services;
using Microsoft.Extensions.Logging;
using ZiggyCreatures.Caching.Fusion;

namespace Core.Infrastructure.Versioning;

public class VersionDiffService(IVersionRepository versionRepository, IFusionCache cache, ILogger<VersionDiffService> logger) : IVersionDiffService
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);
    private const string CacheKeyPrefix = "version-diff";

    public async Task<VersionDiff?> GetDiffAsync(Guid entityId, int fromVersion, int toVersion, CancellationToken cancellationToken = default)
    {
        if (fromVersion == toVersion)
            return new VersionDiff(entityId, fromVersion, toVersion, []);

        var (minVersion, maxVersion) = fromVersion < toVersion
            ? (fromVersion, toVersion)
            : (toVersion, fromVersion);

        var cacheKey = $"{CacheKeyPrefix}:{entityId}:{minVersion}-{maxVersion}";

        return await cache.GetOrSetAsync(
            cacheKey,
            async ct => await ComputeDiffAsync(entityId, minVersion, maxVersion, ct),
            new FusionCacheEntryOptions(CacheDuration),
            cancellationToken);
    }

    private async Task<VersionDiff?> ComputeDiffAsync(Guid entityId, int fromVersion, int toVersion, CancellationToken cancellationToken)
    {
        var fromData = await versionRepository.GetVersionDataAsync(entityId, fromVersion, cancellationToken);
        var toData = await versionRepository.GetVersionDataAsync(entityId, toVersion, cancellationToken);

        if (fromData == null || toData == null)
        {
            logger.LogWarning("Could not find both versions for diff: EntityId={EntityId}, From={From}, To={To}, FoundFrom={FoundFrom}, FoundTo={FoundTo}",
                entityId, fromVersion, toVersion, fromData != null, toData != null);
            return null;
        }

        try
        {
            var fromNode = JsonNode.Parse(fromData);
            var toNode = JsonNode.Parse(toData);

            var diff = fromNode.Diff(toNode, new JsonPatchDeltaFormatter());
            var patchArray = diff as JsonArray ?? [];

            logger.LogDebug("Computed diff for {EntityId} v{From}→v{To}: {OperationCount} operations", entityId, fromVersion, toVersion, patchArray.Count);

            return new VersionDiff(entityId, fromVersion, toVersion, patchArray);
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, "Failed to parse JSON for diff: EntityId={EntityId}, From={From}, To={To}", entityId, fromVersion, toVersion);
            return null;
        }
    }
}
