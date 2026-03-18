namespace Core.Domain.Entities;

public interface ISoftDeleteEntity
{
	DateTime? DeletedAt { get; set; }
	bool IsDeleted { get; set; }
	Guid? DeletedBy { get; set; }
}
