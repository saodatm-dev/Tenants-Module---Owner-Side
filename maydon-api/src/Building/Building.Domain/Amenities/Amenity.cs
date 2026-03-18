using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Building.Domain.Amenities.Events;
using Building.Domain.AmenityCategories;
using Core.Domain.Entities;
using Core.Domain.Languages;

namespace Building.Domain.Amenities;

// Примеры: WiFi, Kitchen, Pool, Air conditioning 
[Table("amenities", Schema = AssemblyReference.Instance)]
public sealed class Amenity : Entity
{
	private Amenity() { }
	public Amenity(
		Guid amenityCategoryId,
		string iconUrl,
		IEnumerable<LanguageValue> translates) : base()
	{
		AmenityCategoryId = amenityCategoryId;
		IconUrl = iconUrl;

		Raise(new UpsertAmenityDomainEvent(this.Id, translates));
	}
	public Guid AmenityCategoryId { get; private set; }
	[Required]
	[MaxLength(500)]
	public string IconUrl { get; private set; }
	public ICollection<AmenityTranslate> Translates { get; private set; }
	public AmenityCategory AmenityCategory { get; private set; }

	public Amenity Update(
		Guid amenityCategoryId,
		string iconUrl,
		IEnumerable<LanguageValue> translates)
	{
		AmenityCategoryId = amenityCategoryId;
		IconUrl = iconUrl;
		Raise(new UpsertAmenityDomainEvent(this.Id, translates));
		return this;
	}

	// to remove related entities
	public Amenity Remove()
	{
		Raise(new RemoveAmenityDomainEvent(this.Id));
		return this;
	}
}
