using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Domain.Currencies.Events;
using Core.Domain.Entities;
using Core.Domain.Languages;

namespace Common.Domain.Currencies;

[Table("currencies", Schema = AssemblyReference.Instance)]
public sealed class Currency : Entity
{
	private Currency() { }
	public Currency(
		string code,
		IEnumerable<LanguageValue> translates) : base()
	{
		Code = code;
		Raise(new UpsertCurrencyDomainEvent(Id, translates));
	}

	[MaxLength(10)]
	public string? Code { get; private set; }
	public short Order { get; private set; }
	public ICollection<CurrencyTranslate> Translates { get; } = new List<CurrencyTranslate>();

	public Currency Update(
		string code,
		IEnumerable<LanguageValue> translates)
	{
		Code = code;
		Raise(new UpsertCurrencyDomainEvent(Id, translates));
		return this;
	}

	public Currency SetOrder(short order)
	{
		Order = order;
		return this;
	}

	public Currency Remove()
	{
		Raise(new RemoveCurrencyDomainEvent(Id));
		return this;
	}
}
