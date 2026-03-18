using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Domain.Entities;

namespace Building.Domain.ProductionTypes;

[Table("production_type_translates", Schema = AssemblyReference.Instance)]
public sealed class ProductionTypeTranslate : Entity
{
	private ProductionTypeTranslate() { }
	public ProductionTypeTranslate(
		Guid productionTypeId,
		Guid languageId,
		string languageShortCode,
		string value) : base()
	{
		ProductionTypeId = productionTypeId;
		LanguageId = languageId;
		LanguageShortCode = languageShortCode;
		Value = value;
	}

	public Guid ProductionTypeId { get; private set; }
	public Guid LanguageId { get; private set; }
	[Required]
	[MaxLength(10)]
	public string LanguageShortCode { get; private set; }
	[Required]
	[MaxLength(200)]
	public string Value { get; private set; }
	public ProductionType ProductionType { get; private set; }

	public ProductionTypeTranslate Update(
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
