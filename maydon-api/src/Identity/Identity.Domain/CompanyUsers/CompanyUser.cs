using System.ComponentModel.DataAnnotations.Schema;
using Core.Domain.Entities;
using Identity.Domain.Accounts.Events;
using Identity.Domain.Companies;
using Identity.Domain.CompanyUsers.Events;
using Identity.Domain.Users;

namespace Identity.Domain.CompanyUsers;

[Table("company_users", Schema = AssemblyReference.Instance)]
public sealed class CompanyUser : Entity
{
	private CompanyUser() { }
	public CompanyUser(
		Guid companyId,
		Guid userId,
		bool isOwner = false) : base()
	{
		CompanyId = companyId;
		UserId = userId;
		IsOwner = isOwner;

		Raise(new UpsertCompanyUserPostDomainEvent(this));
	}

	public Guid CompanyId { get; private set; }
	public Guid UserId { get; private set; }
	public bool IsOwner { get; private set; }
	public bool IsActive { get; private set; } = true;
	public Company Company { get; private set; }
	public User User { get; private set; }

	public CompanyUser Deactivate()
	{
		IsActive = false;
		return this;
	}

	public CompanyUser Activate()
	{
		IsActive = true;
		return this;
	}

	public CompanyUser Remove()
	{
		Raise(new DeleteAccountPreDomainEvent(this.CompanyId, this.UserId));
		Raise(new DeleteCompanyUserPostDomainEvent(this.Id));
		return this;
	}
}

