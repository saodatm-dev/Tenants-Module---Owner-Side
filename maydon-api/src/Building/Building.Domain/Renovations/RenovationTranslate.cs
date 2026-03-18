using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Domain.Entities;

namespace Building.Domain.Renovations;

[Table("renovation_translates", Schema = AssemblyReference.Instance)]
public sealed class RenovationTranslate : Entity
{
	private RenovationTranslate() { }
	public RenovationTranslate(
		Guid renovationId,
		Guid languageId,
		string languageShortCode,
		string value) : base()
	{
		RenovationId = renovationId;
		LanguageId = languageId;
		LanguageShortCode = languageShortCode;
		Value = value;
	}

	public Guid RenovationId { get; private set; }
	public Guid LanguageId { get; private set; }
	[Required]
	[MaxLength(10)]
	public string LanguageShortCode { get; private set; }
	[Required]
	[MaxLength(200)]
	public string Value { get; private set; }
	public Renovation Renovation { get; private set; }

	public RenovationTranslate Update(
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
