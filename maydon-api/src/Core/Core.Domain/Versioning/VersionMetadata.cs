namespace Core.Domain.Versioning;

/// <summary>
/// Metadata about a specific version of an entity.
/// Used for displaying version history to users.
/// </summary>
public sealed class VersionMetadata
{
	public required int VersionNumber { get; init; }
	public required DateTime CreatedAt { get; init; }
	public required Guid? CreatedBy { get; init; }
	public string? ChangeDescription { get; init; }
	public long? DataSize { get; init; }
}
