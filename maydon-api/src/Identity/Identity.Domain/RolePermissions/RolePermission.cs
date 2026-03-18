using System.ComponentModel.DataAnnotations.Schema;
using Core.Domain.Entities;
using Core.Domain.Permissions;
using Identity.Domain.Roles;

namespace Identity.Domain.RolePermissions;

[Table("role_permissions", Schema = AssemblyReference.Instance)]
public sealed class RolePermission : Entity
{
	private RolePermission() { }
	public RolePermission(
		Guid roleId,
		Guid permissionId) : base()
	{
		this.RoleId = roleId;
		this.PermissionId = permissionId;
	}
	public Guid RoleId { get; private set; }
	public Guid PermissionId { get; private set; }
	public Role Role { get; private set; }
	public Permission Permission { get; private set; }
}
