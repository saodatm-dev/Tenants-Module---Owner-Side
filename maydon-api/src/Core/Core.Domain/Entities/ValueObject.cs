namespace Core.Domain.Entities;

public abstract class ValueObject : IEquatable<ValueObject>
{
	protected abstract IEnumerable<object> GetEqualityComponents();

	public override bool Equals(object? obj)
	{
		if (obj == null || obj.GetType() != GetType())
			return false;

		var valueObject = (ValueObject)obj;
		return GetEqualityComponents().SequenceEqual(valueObject.GetEqualityComponents());
	}

	public bool Equals(ValueObject? other) => Equals((object?)other);

	public override int GetHashCode()
	{
		return GetEqualityComponents()
			.Aggregate(1, (current, obj) =>
			{
				unchecked
				{
					return current * 23 + (obj?.GetHashCode() ?? 0);
				}
			});
	}

	public static bool operator ==(ValueObject? left, ValueObject? right)
	{
		if (left is null && right is null) return true;
		if (left is null || right is null) return false;
		return left.Equals(right);
	}

	public static bool operator !=(ValueObject? left, ValueObject? right) => !(left == right);
}
