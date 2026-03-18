using System.Text.Json.Nodes;

namespace Core.Application.Abstractions.Services;

/// <summary>
/// Represents a diff between two entity versions using JSON Patch (RFC 6902).
/// </summary>
public record VersionDiff(Guid EntityId, int FromVersion, int ToVersion, JsonArray Patch);

/// <summary>
/// Service for computing differences between entity versions.
/// </summary>
public interface IVersionDiffService
{
	Task<VersionDiff?> GetDiffAsync(Guid entityId, int fromVersion, int toVersion, CancellationToken cancellationToken = default);
}
