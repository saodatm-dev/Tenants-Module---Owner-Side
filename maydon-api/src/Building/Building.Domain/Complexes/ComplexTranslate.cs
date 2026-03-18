using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Domain.Entities;

namespace Building.Domain.Complexes;

[Table("complex_translates", Schema = AssemblyReference.Instance)]
public sealed class ComplexTranslate : Entity
{
	private ComplexTranslate() { }

	public ComplexTranslate(
		Guid complexId,
		Guid languageId,
		string languageShortCode,
		string value) : base()
	{
		this.ComplexId = complexId;
		this.LanguageId = languageId;
		this.LanguageShortCode = languageShortCode;
		this.Value = value;
	}

	public Guid ComplexId { get; private set; }
	public Guid LanguageId { get; private set; }
	[Required]
	[MaxLength(10)]
	public string LanguageShortCode { get; private set; }
	[Required]
	[MaxLength(500)]
	public string Value { get; private set; }
	public Complex Complex { get; private set; }

	public ComplexTranslate Update(
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
