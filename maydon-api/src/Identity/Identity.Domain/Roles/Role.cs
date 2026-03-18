using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Domain.Entities;
using Core.Domain.RolePermissions;
using Core.Domain.Roles;
using EntityFrameworkCore.EncryptColumn.Attribute;
using Identity.Domain.Roles.Events;

namespace Identity.Domain.Roles;

[Table("roles", Schema = AssemblyReference.Instance)]
public sealed class Role : Entity
{
	private Role() { }
	public Role(
		string name,
		RoleType type,
		Guid? tenantId = null) : base()
	{
		this.Name = name;
		this.TenantId = tenantId;
		this.Type = type;

		Raise(new CreateOrUpdateRolePostDomainEvent(this));
	}
	public Guid? TenantId { get; private set; }
	[EncryptColumn]
	[Required]
	[MaxLength(100)]
	public string Name { get; private set; }
	public RoleType Type { get; private set; }
	public bool IsActive { get; private set; } = true;
	//public ICollection<RolePermission> RolePermissions { get; private set; }

	[NotMapped]
	public Dictionary<string, IEnumerable<RolePermission>> UpsertRolePermissions { get; private set; }

	[NotMapped]
	public Dictionary<string, IEnumerable<RolePermission>> RemoveRolePermissions { get; private set; }

	public Role Update(
		string name,
		RoleType type,
		Guid? tenantId = null)
	{
		this.Name = name;
		this.TenantId = tenantId;
		this.Type = type;
		Raise(new CreateOrUpdateRolePostDomainEvent(this));
		return this;
	}
	public Role Remove()
	{
		Raise(new DeleteRolePostDomainEvent(this.Id));
		return this;
	}
	public Role Activate()
	{
		this.IsActive = true;
		Raise(new CreateOrUpdateRolePostDomainEvent(this));
		return this;
	}
	public Role Deactivate()
	{
		this.IsActive = false;
		Raise(new CreateOrUpdateRolePostDomainEvent(this));
		return this;
	}
	public Role RolePermissionUpsert(
		Dictionary<string, IEnumerable<RolePermission>> moduleRolePermissions)
	{
		UpsertRolePermissions = moduleRolePermissions;
		return this;
	}

	public Role RolePermissionRemove(
		Dictionary<string, IEnumerable<RolePermission>> moduleRolePermissions)
	{
		RemoveRolePermissions = moduleRolePermissions;
		return this;
	}
}
