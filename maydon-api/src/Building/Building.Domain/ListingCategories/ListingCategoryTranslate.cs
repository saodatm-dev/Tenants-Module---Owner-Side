using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Domain.Entities;

namespace Building.Domain.ListingCategories;

[Table("listing_category_translates", Schema = AssemblyReference.Instance)]
public sealed class ListingCategoryTranslate : Entity
{
	private ListingCategoryTranslate() { }
	public ListingCategoryTranslate(
		Guid listingCategoryId,
		Guid languageId,
		string languageShortCode,
		string value) : base()
	{
		ListingCategoryId = listingCategoryId;
		LanguageId = languageId;
		LanguageShortCode = languageShortCode;
		Value = value;
	}

	public Guid ListingCategoryId { get; private set; }
	public Guid LanguageId { get; private set; }
	[Required]
	[MaxLength(10)]
	public string LanguageShortCode { get; private set; }
	[Required]
	[MaxLength(200)]
	public string Value { get; private set; }
	public ListingCategory ListingCategory { get; private set; }

	public ListingCategoryTranslate Update(
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
