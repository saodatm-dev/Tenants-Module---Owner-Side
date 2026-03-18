using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Domain.Languages;
using Core.Domain.Entities;

namespace Common.Domain.Districts;

[Table("district_translates", Schema = AssemblyReference.Instance)]
public sealed class DistrictTranslate : Entity
{
	private DistrictTranslate() { }

	public DistrictTranslate(
		Guid districtId,
		Guid languageId,
		string languageShortCode,
		string value) : base()
	{
		this.DistrictId = districtId;
		this.LanguageId = languageId;
		this.LanguageShortCode = languageShortCode;
		this.Value = value;
	}

	public Guid DistrictId { get; private set; }
	public Guid LanguageId { get; private set; }
	[Required]
	[MaxLength(10)]
	public string LanguageShortCode { get; private set; }
	[Required]
	[MaxLength(200)]
	public string Value { get; private set; }
	public District District { get; private set; }
	public Language Language { get; private set; }

	public DistrictTranslate Update(
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
