using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Domain.Entities;
using EntityFrameworkCore.EncryptColumn.Attribute;
using Identity.Domain.BankProperties.Events;

namespace Identity.Domain.BankProperties;

[Table("bank_properties", Schema = AssemblyReference.Instance)]
public sealed class BankProperty : Entity
{
	private BankProperty() { }
	public BankProperty(
		Guid tenantId,
		Guid bankId,
		string bankName,
		string mfo,
		string accountNumber,
		bool isMain,
		bool isPublic) : base()
	{
		this.TenantId = tenantId;
		this.BankId = bankId;
		this.BankName = bankName;
		this.BankMFO = mfo;
		this.AccountNumber = accountNumber;
		this.IsMain = isMain;
		this.IsPublic = isPublic;
	}

	public Guid TenantId { get; private set; }
	public Guid BankId { get; private set; }
	[Required]
	[MaxLength(200)]
	public string BankName { get; private set; }
	[Required]
	[MaxLength(10)]
	public string BankMFO { get; private set; }
	[EncryptColumn]
	[Required]
	[MaxLength(30)]
	public string AccountNumber { get; private set; }  //varchar(20)
	public bool IsMain { get; private set; }
	public bool IsPublic { get; private set; } = true;

	public BankProperty Update(
		Guid bankId,
		string bankName,
		string mfo,
		string accountNumber,
		bool isMain,
		bool isPublic)
	{
		this.BankId = bankId;
		this.BankName = bankName;
		this.BankMFO = mfo;
		this.AccountNumber = accountNumber;
		this.IsMain = isMain;
		this.IsPublic = isPublic;

		return this;
	}
	public BankProperty Remove()
	{
		if (this.IsMain)
			Raise(new RemoveMainBankPropertyDomainEvent(this.Id, this.TenantId));

		return this;
	}
	public BankProperty EnableMain()
	{
		this.IsMain = true;
		return this;
	}
	public BankProperty DisableMain()
	{
		this.IsMain = false;
		return this;
	}
}
