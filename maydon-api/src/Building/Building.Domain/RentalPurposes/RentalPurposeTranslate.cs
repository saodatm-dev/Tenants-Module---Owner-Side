using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Domain.Entities;

namespace Building.Domain.RentalPurposes;

[Table("rental_purpose_translates", Schema = AssemblyReference.Instance)]
public sealed class RentalPurposeTranslate : Entity
{
	private RentalPurposeTranslate() { }
	public RentalPurposeTranslate(
		Guid rentalPurposeId,
		Guid languageId,
		string languageShortCode,
		string value) : base()
	{
		RentalPurposeId = rentalPurposeId;
		LanguageId = languageId;
		LanguageShortCode = languageShortCode;
		Value = value;
	}

	public Guid RentalPurposeId { get; private set; }
	public Guid LanguageId { get; private set; }
	[Required]
	[MaxLength(10)]
	public string LanguageShortCode { get; private set; }
	[Required]
	[MaxLength(200)]
	public string Value { get; private set; }
	public RentalPurpose RentalPurpose { get; private set; }

	public RentalPurposeTranslate Update(
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
