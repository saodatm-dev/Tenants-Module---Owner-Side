using System.ComponentModel.DataAnnotations.Schema;
using Building.Domain.LandCategories.Events;
using Core.Domain.Entities;
using Core.Domain.Languages;

namespace Building.Domain.LandCategories;

[Table("land_categories", Schema = AssemblyReference.Instance)]
public sealed class LandCategory : Entity
{
	private LandCategory() { }
	public LandCategory(
		IEnumerable<LanguageValue> translates) : base()
	{
		Raise(new CreateOrUpdateLandCategoryDomainEvent(this.Id, translates));
	}
	public ICollection<LandCategoryTranslate> Translates { get; private set; }
	public bool IsActive { get; private set; } = true;

	public LandCategory Update(
		IEnumerable<LanguageValue> translates)
	{
		Raise(new CreateOrUpdateLandCategoryDomainEvent(this.Id, translates));
		return this;
	}

	public LandCategory Remove()
	{
		Raise(new RemoveLandCategoryDomainEvent(this.Id));
		return this;
	}

	public LandCategory Activate()
	{
		this.IsActive = true;
		return this;
	}

	public LandCategory Deactivate()
	{
		this.IsActive = false;
		return this;
	}
}
