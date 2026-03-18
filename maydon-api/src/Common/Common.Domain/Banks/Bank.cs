using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Domain.Banks.Events;
using Core.Domain.Entities;
using Core.Domain.Languages;

namespace Common.Domain.Banks;

[Table("banks", Schema = AssemblyReference.Instance)]
public sealed class Bank : Entity
{
	private Bank() { }
	public Bank(
		string mfo,
		string? tin,
		string? phoneNumber,
		string? email,
		string? website,
		string address,
		IEnumerable<LanguageValue> translates) : base()
	{
		Mfo = mfo;
		Tin = tin;
		PhoneNumber = phoneNumber;
		Email = email;
		Website = website;
		Address = address;

		Raise(new CreateOrUpdateBankDomainEvent(Id, translates));
	}
	[Required]
	[MaxLength(20)]
	public string Mfo { get; private set; }
	[MaxLength(20)]
	public string? Tin { get; private set; }
	[MaxLength(20)]
	public string? PhoneNumber { get; private set; }
	[MaxLength(100)]
	public string? Email { get; private set; }
	[MaxLength(200)]
	public string? Website { get; private set; }
	[MaxLength(500)]
	public string? Address { get; private set; }
	public short Order { get; private set; }
	public ICollection<BankTranslate> Translates { get; } = new List<BankTranslate>();
	public Bank Update(
		string mfo,
		string? tin,
		string? phoneNumber,
		string? email,
		string? website,
		string address,
		IEnumerable<LanguageValue> translates)
	{
		Mfo = mfo;
		Tin = tin;
		PhoneNumber = phoneNumber;
		Email = email;
		Website = website;
		Address = address;
		Raise(new CreateOrUpdateBankDomainEvent(Id, translates));
		return this;
	}

	public Bank SetOrder(short order)
	{
		Order = order;
		return this;
	}

	public Bank Remove()
	{
		Raise(new RemoveBankDomainEvent(Id));
		return this;
	}
}
