using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Domain.Entities;

namespace Building.Domain.MeterTypes;

[Table("meter_type_translates", Schema = AssemblyReference.Instance)]
public sealed class MeterTypeTranslate : Entity
{
	private MeterTypeTranslate() { }

	public MeterTypeTranslate(
		Guid meterTypeId,
		MeterTypeField field,
		Guid languageId,
		string languageShortCode,
		string value) : base()
	{
		this.MeterTypeId = meterTypeId;
		this.Field = field;
		this.LanguageId = languageId;
		this.LanguageShortCode = languageShortCode;
		this.Value = value;
	}

	public Guid MeterTypeId { get; private set; }
	public MeterTypeField Field { get; private set; }
	public Guid LanguageId { get; private set; }
	[Required]
	[MaxLength(10)]
	public string LanguageShortCode { get; private set; }
	[Required]
	[MaxLength(200)]
	public string Value { get; private set; }
	public MeterType MeterType { get; private set; }

	public MeterTypeTranslate Update(
		MeterTypeField field,
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
