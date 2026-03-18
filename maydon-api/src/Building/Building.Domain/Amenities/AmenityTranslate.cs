using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Domain.Entities;

namespace Building.Domain.Amenities;

[Table("amenity_translates", Schema = AssemblyReference.Instance)]
public sealed class AmenityTranslate : Entity
{
	private AmenityTranslate() { }
	public AmenityTranslate(
		Guid amenityId,
		Guid languageId,
		string languageShortCode,
		string value) : base()
	{
		AmenityId = amenityId;
		LanguageId = languageId;
		LanguageShortCode = languageShortCode;
		Value = value;
	}

	public Guid AmenityId { get; private set; }
	public Guid LanguageId { get; private set; }
	[Required]
	[MaxLength(10)]
	public string LanguageShortCode { get; private set; }
	[Required]
	[MaxLength(200)]
	public string Value { get; private set; }
	public Amenity Amenity { get; private set; }

	public AmenityTranslate Update(
		Guid languageId,
		string languageShortCode,
		string value)
	{
		LanguageId = languageId;
		LanguageShortCode = languageShortCode;
		Value = value;
		return this;
	}
}
