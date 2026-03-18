using Common.Domain;
using Core.Domain.Permissions;
using Core.Domain.RolePermissions;
using Maydon.Administration.Host.Abstractions;

namespace Maydon.Administration.Host.Permissions.Common;

internal sealed class BankPermissions : IPermission
{
	internal static string GroupName => AssemblyReference.Instance;
	internal static PermissionType PermissionBankCreate = new PermissionType("banks:create");
	internal static PermissionType PermissionBankUpdate = new PermissionType("banks:update");
	internal static PermissionType PermissionBankRemove = new PermissionType("banks:remove");


	internal static IEnumerable<RolePermissionValue> DefaultRolePermissions = [
		new RolePermissionValue(PermissionBankCreate.PermissionName,Core.Domain.Roles.RoleType.System),
		new RolePermissionValue(PermissionBankUpdate.PermissionName,Core.Domain.Roles.RoleType.System),
		new RolePermissionValue(PermissionBankRemove.PermissionName,Core.Domain.Roles.RoleType.System)];
}
