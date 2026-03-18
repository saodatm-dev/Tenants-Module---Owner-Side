using Core.Domain.Permissions;
using Core.Domain.RolePermissions;
using Identity.Domain;
using Maydon.Host.Abstractions;

namespace Maydon.Host.Permissions.Identity;

internal sealed class BindingPermissions : IPermission
{
	internal static string GroupName => AssemblyReference.Instance;

	internal static PermissionType PermissionBindingOneId = new PermissionType("bindings:oneid");
	internal static PermissionType PermissionBindingEimzo = new PermissionType("bindings:eimzo");


	internal static IEnumerable<RolePermissionValue> DefaultRolePermissions = [
		new RolePermissionValue(PermissionBindingOneId.PermissionName,Core.Domain.Roles.RoleType.Owner),
		new RolePermissionValue(PermissionBindingEimzo.PermissionName,Core.Domain.Roles.RoleType.Owner),
		new RolePermissionValue(PermissionBindingOneId.PermissionName,Core.Domain.Roles.RoleType.Client),
		new RolePermissionValue(PermissionBindingEimzo.PermissionName,Core.Domain.Roles.RoleType.Client)];
}
