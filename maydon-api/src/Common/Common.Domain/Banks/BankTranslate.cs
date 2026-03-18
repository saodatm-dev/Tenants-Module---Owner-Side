using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Domain.Entities;

namespace Common.Domain.Banks;

[Table("bank_translates", Schema = AssemblyReference.Instance)]
public sealed class BankTranslate : Entity
{
	private BankTranslate() { }

	public BankTranslate(
		Guid bankId,
		Guid languageId,
		string languageShortCode,
		string value) : base()
	{
		BankId = bankId;
		LanguageId = languageId;
		LanguageShortCode = languageShortCode;
		Value = value;
	}

	public Guid BankId { get; private set; }
	public Guid LanguageId { get; private set; }
	[Required]
	[MaxLength(10)]
	public string LanguageShortCode { get; private set; }
	[Required]
	[MaxLength(200)]
	public string Value { get; private set; }
	public Bank Bank { get; private set; }

	public BankTranslate Update(
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
