using System.ComponentModel.DataAnnotations.Schema;
using Core.Domain.Entities;
using Identity.Domain.Companies;
using Identity.Domain.Roles;
using Identity.Domain.Users;

namespace Identity.Domain.Accounts;

[Table("accounts", Schema = AssemblyReference.Instance)]
public sealed class Account : Entity
{
	private Account() { }
	public Account(
		Guid tenantId,
		Guid userId,
		AccountType type = AccountType.Client,
		bool isDefault = false) : base()
	{
		TenantId = tenantId;
		UserId = userId;
		Type = type;
		IsDefault = isDefault;
	}

	public Guid TenantId { get; private set; }
	public Guid UserId { get; private set; }
	public Guid RoleId { get; private set; }
	public AccountType Type { get; private set; }
	public bool IsDefault { get; private set; }
	public bool IsActive { get; private set; } = true;
	//[ForeignKey(nameof(TenantId))]
	public Company? Company { get; private set; }
	public User User { get; private set; }
	public Role Role { get; private set; }
	public Account Update(Account account)
	{
		this.RoleId = account.RoleId;
		this.Type = account.Type;
		this.IsActive = account.IsActive;
		return this;
	}
	public Account Default()
	{
		this.IsDefault = true;
		return this;
	}
	public Account NonDefault()
	{
		this.IsDefault = false;
		return this;
	}
	public Account ChangeRole(Guid roleId)
	{
		RoleId = roleId;
		return this;
	}
	public Account Activate()
	{
		this.IsActive = true;
		return this;
	}
	public Account Deactivate()
	{
		this.IsActive = false;
		return this;
	}
}
