using Core.Domain.Permissions;
using Core.Domain.RolePermissions;
using Identity.Domain;
using Maydon.Host.Abstractions;

namespace Maydon.Host.Permissions.Identity;

internal sealed class AccountPermissions : IPermission
{
	internal static string GroupName => AssemblyReference.Instance;

	internal static PermissionType PermissionAccountMy = new PermissionType("accounts:my");
	internal static PermissionType PermissionAccountChange = new PermissionType("accounts:change");
	internal static PermissionType PermissionAccountCreateOwner = new PermissionType("accounts:create-owner");
	internal static PermissionType PermissionAccountCreateClient = new PermissionType("accounts:create-client");


	internal static IEnumerable<RolePermissionValue> DefaultRolePermissions = [
		new RolePermissionValue(PermissionAccountMy.PermissionName,Core.Domain.Roles.RoleType.Owner),
		new RolePermissionValue(PermissionAccountChange.PermissionName,Core.Domain.Roles.RoleType.Owner),
		new RolePermissionValue(PermissionAccountCreateClient.PermissionName,Core.Domain.Roles.RoleType.Owner),
		new RolePermissionValue(PermissionAccountMy.PermissionName,Core.Domain.Roles.RoleType.Client),
		new RolePermissionValue(PermissionAccountChange.PermissionName,Core.Domain.Roles.RoleType.Client),
		new RolePermissionValue(PermissionAccountCreateOwner.PermissionName,Core.Domain.Roles.RoleType.Client)];
}
