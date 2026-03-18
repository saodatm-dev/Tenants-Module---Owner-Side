using System.ComponentModel.DataAnnotations.Schema;
using Building.Domain.AmenityCategories.Events;
using Core.Domain.Entities;
using Core.Domain.Languages;

namespace Building.Domain.AmenityCategories;

// Примеры: WiFi, Kitchen, Pool, Air conditioning 
[Table("amenity_categories", Schema = AssemblyReference.Instance)]
public sealed class AmenityCategory : Entity
{
	private AmenityCategory() { }
	public AmenityCategory(IEnumerable<LanguageValue> translates) : base()
	{
		Raise(new UpsertAmenityCategoryDomainEvent(this.Id, translates));
	}
	public ICollection<AmenityCategoryTranslate> Translates { get; private set; }
	public AmenityCategory Update(IEnumerable<LanguageValue> translates)
	{
		Raise(new UpsertAmenityCategoryDomainEvent(this.Id, translates));
		return this;
	}
	public AmenityCategory Remove()
	{
		Raise(new RemoveAmenityCategoryDomainEvent(this.Id));
		return this;
	}
}
