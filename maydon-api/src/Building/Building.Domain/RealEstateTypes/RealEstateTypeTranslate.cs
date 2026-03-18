using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Domain.Entities;

namespace Building.Domain.RealEstateTypes;

[Table("real_estate_type_translates", Schema = AssemblyReference.Instance)]
public sealed class RealEstateTypeTranslate : Entity
{
	private RealEstateTypeTranslate() { }

	public RealEstateTypeTranslate(
		Guid realEstateTypeId,
		RealEstateTypeField field,
		Guid languageId,
		string languageShortCode,
		string value) : base()
	{
		this.RealEstateTypeId = realEstateTypeId;
		this.Field = field;
		this.LanguageId = languageId;
		this.LanguageShortCode = languageShortCode;
		this.Value = value;
	}
	public Guid RealEstateTypeId { get; private set; }
	public RealEstateTypeField Field { get; private set; }
	public Guid LanguageId { get; private set; }
	[Required]
	[MaxLength(10)]
	public string LanguageShortCode { get; private set; }
	[Required]
	[MaxLength(500)]
	public string Value { get; private set; }
	public RealEstateType RealEstateType { get; private set; }

	public RealEstateTypeTranslate Update(
		RealEstateTypeField field,
		Guid languageId,
		string languageShortCode,
		string value)
	{
		this.Field = field;
		this.LanguageId = languageId;
		this.LanguageShortCode = languageShortCode;
		this.Value = value;

		return this;
	}
}
