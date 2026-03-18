using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Building.Domain.Buildings;
using Building.Domain.Categories.Events;
using Core.Domain.Entities;
using Core.Domain.Languages;

namespace Building.Domain.Categories;

[Table("categories", Schema = AssemblyReference.Instance)]
public sealed class Category : Entity
{
	private Category() { }
	public Category(
		BuildingType buildingType,
		string iconUrl,
		IEnumerable<LanguageValue> translates,
		Guid? parentId = null) : base()
	{
		ParentId = parentId;
		BuildingType = buildingType;
		IconUrl = iconUrl;
		Raise(new UpsertCategoryDomainEvent(this.Id, translates));
	}

	public Guid? ParentId { get; private set; }
	public BuildingType BuildingType { get; private set; }
	[Required]
	[MaxLength(500)]
	public string IconUrl { get; private set; }
	public ICollection<CategoryTranslate> Translates { get; private set; }
	public bool IsActive { get; private set; } = true;

	public Category Update(
		BuildingType buildingType,
		string iconUrl,
		IEnumerable<LanguageValue> translates)
	{
		BuildingType = buildingType;
		IconUrl = iconUrl;
		Raise(new UpsertCategoryDomainEvent(this.Id, translates));
		return this;
	}
	// to remove regions translates
	public Category Remove()
	{
		Raise(new RemoveCategoryDomainEvent(this.Id));
		return this;
	}
	public Category Activate()
	{
		this.IsActive = true;
		return this;
	}
	public Category Deactivate()
	{
		this.IsActive = false;
		return this;
	}
}
