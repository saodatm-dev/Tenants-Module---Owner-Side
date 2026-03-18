namespace Core.Domain.Entities;

public interface IModifiableEntity
{
	Guid? CreatedBy { get; }
	Guid? UpdatedBy { get; }
}
