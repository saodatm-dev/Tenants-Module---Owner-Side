using Core.Domain.Permissions;
using Core.Domain.RolePermissions;
using Identity.Domain;
using Maydon.Administration.Host.Abstractions;

namespace Maydon.Administration.Host.Permissions.Identity;

internal sealed class AccountPermissions : IPermission
{
	internal static string GroupName => AssemblyReference.Instance;

	internal static PermissionType PermissionAccountList = new PermissionType("accounts:list");

	internal static IEnumerable<RolePermissionValue> DefaultRolePermissions = [
		new RolePermissionValue(PermissionAccountList.PermissionName,Core.Domain.Roles.RoleType.System)];
}
