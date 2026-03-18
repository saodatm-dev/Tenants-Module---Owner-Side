namespace Core.Domain.Entities;

/// <summary>
/// Marks entities that support versioning and optimistic concurrency.
/// </summary>
public interface IVersionedEntity
{
	Guid Id { get; }
	int CurrentVersion { get; set; }
}
