using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Domain.Entities;

namespace Building.Domain.Listings;

[Table("listing_translates", Schema = AssemblyReference.Instance)]
public sealed class ListingTranslate : Entity
{
	private ListingTranslate() { }
	public ListingTranslate(
		Guid listingId,
		Guid languageId,
		string languageShortCode,
		string value) : base()
	{
		ListingId = listingId;
		LanguageId = languageId;
		LanguageShortCode = languageShortCode;
		Value = value;
	}

	public Guid ListingId { get; private set; }
	public Guid LanguageId { get; private set; }
	[Required]
	[MaxLength(10)]
	public string LanguageShortCode { get; private set; }
	[Required]
	[MaxLength(2000)]
	public string Value { get; private set; }
	public Listing Listing { get; private set; }

	public ListingTranslate Update(
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
