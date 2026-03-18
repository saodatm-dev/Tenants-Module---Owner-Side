using System.ComponentModel.DataAnnotations.Schema;
using Building.Domain.Categories;
using Core.Domain.Entities;

namespace Building.Domain.BuildingCategories;

[Table("building_categories", Schema = AssemblyReference.Instance)]
public sealed class BuildingCategory : Entity
{
	private BuildingCategory() { }
	public BuildingCategory(
		Guid buildingId,
		Guid categoryId) : base() =>
		(BuildingId, CategoryId) = (buildingId, categoryId);

	public Guid BuildingId { get; private set; }
	public Guid CategoryId { get; private set; }
	public Buildings.Building Building { get; private set; }
	public Category Category { get; private set; }

	public BuildingCategory Update(Guid categoryId)
	{
		CategoryId = categoryId;
		return this;
	}
}

