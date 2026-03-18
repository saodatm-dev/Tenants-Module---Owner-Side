using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Domain.Entities;

namespace Building.Domain.Buildings;

[Table("building_translates", Schema = AssemblyReference.Instance)]
public sealed class BuildingTranslate : Entity
{
	private BuildingTranslate() { }

	public BuildingTranslate(
		Guid buildingId,
		Guid languageId,
		string languageShortCode,
		string value) : base()
	{
		this.BuildingId = buildingId;
		this.LanguageId = languageId;
		this.LanguageShortCode = languageShortCode;
		this.Value = value;
	}

	public Guid BuildingId { get; private set; }
	public Guid LanguageId { get; private set; }
	[Required]
	[MaxLength(10)]
	public string LanguageShortCode { get; private set; }
	[Required]
	[MaxLength(500)]
	public string Value { get; private set; }
	public Building Building { get; private set; }

	public BuildingTranslate Update(
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
