using Common.Domain;
using Core.Domain.Permissions;
using Core.Domain.RolePermissions;
using Maydon.Administration.Host.Abstractions;

namespace Maydon.Administration.Host.Permissions.Common;

internal sealed class CurrencyPermissions : IPermission
{
	internal static string GroupName => AssemblyReference.Instance;
	internal static PermissionType PermissionCurrencyCreate = new PermissionType("currencies:create");
	internal static PermissionType PermissionCurrencyUpdate = new PermissionType("currencies:update");
	internal static PermissionType PermissionCurrencyRemove = new PermissionType("currencies:remove");


	internal static IEnumerable<RolePermissionValue> DefaultRolePermissions = [
		new RolePermissionValue(PermissionCurrencyCreate.PermissionName,Core.Domain.Roles.RoleType.System),
		new RolePermissionValue(PermissionCurrencyUpdate.PermissionName,Core.Domain.Roles.RoleType.System),
		new RolePermissionValue(PermissionCurrencyRemove.PermissionName,Core.Domain.Roles.RoleType.System)];
}
