using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Domain.Entities;

namespace Building.Domain.RealEstates;

[Table("real_estate_translates", Schema = AssemblyReference.Instance)]
public sealed class RealEstateTranslate : Entity
{
	private RealEstateTranslate() { }

	public RealEstateTranslate(
		Guid realEstateId,
		Guid languageId,
		string languageShortCode,
		string value) : base()
	{
		this.RealEstateId = realEstateId;
		this.LanguageId = languageId;
		this.LanguageShortCode = languageShortCode;
		this.Value = value;
	}

	public Guid RealEstateId { get; private set; }
	public Guid LanguageId { get; private set; }
	[Required]
	[MaxLength(10)]
	public string LanguageShortCode { get; private set; }
	[Required]
	[MaxLength(500)]
	public string Value { get; private set; }
	public RealEstate RealEstate { get; private set; }

	public RealEstateTranslate Update(
		Guid languageId,
		string languageShortCode,
		string value)
	{
		this.LanguageId = languageId;
		this.LanguageShortCode = languageShortCode;
		this.Value = value;

		return this;
	}
}
