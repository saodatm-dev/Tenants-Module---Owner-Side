using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Building.Domain.Buildings;
using Building.Domain.ListingCategories.Events;
using Core.Domain.Entities;
using Core.Domain.Languages;

namespace Building.Domain.ListingCategories;

[Table("listing_categories", Schema = AssemblyReference.Instance)]
public sealed class ListingCategory : Entity
{
	private ListingCategory() { }
	public ListingCategory(
		BuildingType buildingType,
		string iconUrl,
		IEnumerable<LanguageValue> translates,
		bool showInMain = false,
		Guid? parentId = null) : base()
	{
		ParentId = parentId;
		BuildingType = buildingType;
		IconUrl = iconUrl;
		ShowInMain = showInMain;
		Raise(new UpsertListingCategoryDomainEvent(this.Id, translates));
	}

	public Guid? ParentId { get; private set; }
	public BuildingType BuildingType { get; private set; }
	[Required]
	[MaxLength(500)]
	public string IconUrl { get; private set; }
	public ICollection<ListingCategoryTranslate> Translates { get; private set; }
	public long Order { get; private set; }
	public bool ShowInMain { get; private set; }
	public bool IsActive { get; private set; } = true;

	public ListingCategory Update(
			BuildingType buildingType,
			string iconUrl,
			IEnumerable<LanguageValue> translates,
			bool showInMain = false,
			Guid? parentId = null)
	{
		ParentId = parentId;
		BuildingType = buildingType;
		IconUrl = iconUrl;
		ShowInMain = showInMain;
		Raise(new UpsertListingCategoryDomainEvent(this.Id, translates));
		return this;
	}
	// to remove regions translates
	public ListingCategory Remove()
	{
		Raise(new RemoveListingCategoryDomainEvent(this.Id));
		return this;
	}
	public ListingCategory Activate()
	{
		this.IsActive = true;
		return this;
	}
	public ListingCategory Deactivate()
	{
		this.IsActive = false;
		return this;
	}
}
