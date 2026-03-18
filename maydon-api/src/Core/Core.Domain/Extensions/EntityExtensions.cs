using Core.Domain.Entities;

namespace Core.Domain.Extensions;

/// <summary>
/// Extension methods for entity operations
/// </summary>
public static class EntityExtensions
{
	/// <summary>
	/// Marks an entity as soft deleted
	/// </summary>
	public static void SoftDelete<T>(this T entity, Guid? deletedBy = null) where T : ISoftDeleteEntity
	{
		entity.IsDeleted = true;
		entity.DeletedAt = DateTime.UtcNow;
		entity.DeletedBy = deletedBy;
	}

	/// <summary>
	/// Restores a soft-deleted entity
	/// </summary>
	public static void Restore<T>(this T entity) where T : ISoftDeleteEntity
	{
		entity.IsDeleted = false;
		entity.DeletedAt = null;
		entity.DeletedBy = null;
	}
}
