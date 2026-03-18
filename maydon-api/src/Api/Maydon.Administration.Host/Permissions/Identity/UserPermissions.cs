using Core.Domain.Permissions;
using Core.Domain.RolePermissions;
using Identity.Domain;
using Maydon.Administration.Host.Abstractions;

namespace Maydon.Administration.Host.Permissions.Identity;

internal sealed class UserPermissions : IPermission
{
	internal static string GroupName => AssemblyReference.Instance;

	internal static PermissionType PermissionUserList = new PermissionType("users:list");
	internal static PermissionType PermissionUserGetById = new PermissionType("users:get-by-id");

	internal static IEnumerable<RolePermissionValue> DefaultRolePermissions = [
		new RolePermissionValue(PermissionUserList.PermissionName,Core.Domain.Roles.RoleType.System),
		new RolePermissionValue(PermissionUserGetById.PermissionName,Core.Domain.Roles.RoleType.System)];
}
