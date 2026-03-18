using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Domain.Languages;
using Core.Domain.Entities;

namespace Common.Domain.Regions;

[Table("region_translates", Schema = AssemblyReference.Instance)]
public sealed class RegionTranslate : Entity
{
	private RegionTranslate() { }
	public RegionTranslate(
		Guid regionId,
		Guid languageId,
		string languageShortCode,
		string value) : base()
	{
		RegionId = regionId;
		LanguageId = languageId;
		LanguageShortCode = languageShortCode;
		Value = value;
	}
	public Guid RegionId { get; private set; }
	public Guid LanguageId { get; private set; }
	[Required]
	[MaxLength(10)]
	public string LanguageShortCode { get; private set; }
	[Required]
	[MaxLength(200)]
	public string Value { get; private set; }
	public Region Region { get; private set; }
	public Language Language { get; private set; }

	public RegionTranslate Update(
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
