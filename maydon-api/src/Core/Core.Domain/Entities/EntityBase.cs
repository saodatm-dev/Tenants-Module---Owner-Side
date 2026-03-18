namespace Core.Domain.Entities;

/// <summary>
/// Generic entity base class for entities with custom key types.
/// Used by Document/Didox modules. For Guid-keyed entities with audit fields, use <see cref="Entity"/>.
/// </summary>
public abstract class EntityBase<TKey> : IEquatable<EntityBase<TKey>> where TKey : notnull
{
	public TKey Id { get; protected set; } = default!;

	public bool Equals(EntityBase<TKey>? other)
	{
		if (other is null) return false;
		if (ReferenceEquals(this, other)) return true;
		return EqualityComparer<TKey>.Default.Equals(Id, other.Id);
	}

	public override bool Equals(object? obj)
	{
		if (obj is null) return false;
		if (ReferenceEquals(this, obj)) return true;
		if (obj.GetType() != GetType()) return false;
		return Equals((EntityBase<TKey>)obj);
	}

	public override int GetHashCode() => EqualityComparer<TKey>.Default.GetHashCode(Id);

	public static bool operator ==(EntityBase<TKey>? left, EntityBase<TKey>? right) =>
		Equals(left, right);

	public static bool operator !=(EntityBase<TKey>? left, EntityBase<TKey>? right) =>
		!Equals(left, right);
}
