using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Domain.Entities;

namespace Building.Domain.AmenityCategories;

[Table("amenity_category_translates", Schema = AssemblyReference.Instance)]
public sealed class AmenityCategoryTranslate : Entity
{
	private AmenityCategoryTranslate() { }
	public AmenityCategoryTranslate(
		Guid amenityCategoryId,
		Guid languageId,
		string languageShortCode,
		string value) : base()
	{
		AmenityCategoryId = amenityCategoryId;
		LanguageId = languageId;
		LanguageShortCode = languageShortCode;
		Value = value;
	}

	public Guid AmenityCategoryId { get; private set; }
	public Guid LanguageId { get; private set; }
	[Required]
	[MaxLength(10)]
	public string LanguageShortCode { get; private set; }
	[Required]
	[MaxLength(200)]
	public string Value { get; private set; }
	public AmenityCategory AmenityCategory { get; private set; }

	public AmenityCategoryTranslate Update(
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
