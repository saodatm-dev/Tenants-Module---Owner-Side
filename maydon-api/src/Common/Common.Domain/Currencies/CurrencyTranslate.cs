using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Domain.Languages;
using Core.Domain.Entities;

namespace Common.Domain.Currencies;

[Table("currency_translates", Schema = AssemblyReference.Instance)]
public sealed class CurrencyTranslate : Entity
{
	private CurrencyTranslate() { }
	public CurrencyTranslate(
		Guid currencyId,
		Guid languageId,
		string languageShortCode,
		string value) : base()
	{
		CurrencyId = currencyId;
		LanguageId = languageId;
		LanguageShortCode = languageShortCode;
		Value = value;
	}

	public Guid CurrencyId { get; private set; }
	public Guid LanguageId { get; private set; }
	[Required]
	[MaxLength(10)]
	public string LanguageShortCode { get; private set; }
	[Required]
	[MaxLength(100)]
	public string Value { get; private set; }
	public Currency Currency { get; private set; }
	public Language Language { get; private set; }

	public CurrencyTranslate Update(
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
