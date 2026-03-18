using Core.Domain.Permissions;
using Core.Domain.RolePermissions;
using Identity.Domain;
using Maydon.Host.Abstractions;

namespace Maydon.Host.Permissions.Identity;

internal sealed class UserPermissions : IPermission
{
	internal static string GroupName => AssemblyReference.Instance;

	internal static PermissionType PermissionUserList = new PermissionType("users:list");
	internal static PermissionType PermissionUserGetById = new PermissionType("users:get-by-id");
	internal static PermissionType PermissionUserCreate = new PermissionType("users:create");
	internal static PermissionType PermissionUserUpdate = new PermissionType("users:update");
	internal static PermissionType PermissionUserLogo = new PermissionType("users:logo");
	internal static PermissionType PermissionUserRemove = new PermissionType("users:remove");

	internal static IEnumerable<RolePermissionValue> DefaultRolePermissions = [
		new RolePermissionValue(PermissionUserList.PermissionName,Core.Domain.Roles.RoleType.Owner),
		new RolePermissionValue(PermissionUserGetById.PermissionName,Core.Domain.Roles.RoleType.Owner),
		new RolePermissionValue(PermissionUserCreate.PermissionName,Core.Domain.Roles.RoleType.Owner),
		new RolePermissionValue(PermissionUserUpdate.PermissionName,Core.Domain.Roles.RoleType.Owner),
		new RolePermissionValue(PermissionUserLogo.PermissionName,Core.Domain.Roles.RoleType.Owner),
		new RolePermissionValue(PermissionUserRemove.PermissionName,Core.Domain.Roles.RoleType.Owner),
		new RolePermissionValue(PermissionUserList.PermissionName,Core.Domain.Roles.RoleType.Client),
		new RolePermissionValue(PermissionUserGetById.PermissionName,Core.Domain.Roles.RoleType.Client),
		new RolePermissionValue(PermissionUserCreate.PermissionName,Core.Domain.Roles.RoleType.Client),
		new RolePermissionValue(PermissionUserUpdate.PermissionName,Core.Domain.Roles.RoleType.Client),
		new RolePermissionValue(PermissionUserLogo.PermissionName,Core.Domain.Roles.RoleType.Client),
		new RolePermissionValue(PermissionUserRemove.PermissionName,Core.Domain.Roles.RoleType.Client)];
}
