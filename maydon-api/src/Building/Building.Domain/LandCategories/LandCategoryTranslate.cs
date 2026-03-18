using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Domain.Entities;

namespace Building.Domain.LandCategories;

[Table("land_category_translates", Schema = AssemblyReference.Instance)]
public sealed class LandCategoryTranslate : Entity
{
	private LandCategoryTranslate() { }
	public LandCategoryTranslate(
		Guid landCategoryId,
		Guid languageId,
		string languageShortCode,
		string value) : base()
	{
		LandCategoryId = landCategoryId;
		LanguageId = languageId;
		LanguageShortCode = languageShortCode;
		Value = value;
	}

	public Guid LandCategoryId { get; private set; }
	public Guid LanguageId { get; private set; }
	[Required]
	[MaxLength(10)]
	public string LanguageShortCode { get; private set; }
	[Required]
	[MaxLength(200)]
	public string Value { get; private set; }
	public LandCategory LandCategory { get; private set; }

	public LandCategoryTranslate Update(
		Guid languageId,
		string languageShortCode,
		string value
	)
	{
		LanguageId = languageId;
		LanguageShortCode = languageShortCode;
		Value = value;
		return this;
	}

}
