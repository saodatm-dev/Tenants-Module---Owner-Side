using Common.Domain;
using Core.Domain.Permissions;
using Core.Domain.RolePermissions;
using Maydon.Administration.Host.Abstractions;

namespace Maydon.Administration.Host.Permissions.Common;

internal sealed class LanguagePermissions : IPermission
{
	internal static string GroupName => AssemblyReference.Instance;
	internal static PermissionType PermissionLanguageCreate = new PermissionType("languages:create");
	internal static PermissionType PermissionLanguageUpdate = new PermissionType("languages:update");
	internal static PermissionType PermissionLanguageRemove = new PermissionType("languages:remove");


	internal static IEnumerable<RolePermissionValue> DefaultRolePermissions = [
		new RolePermissionValue(PermissionLanguageCreate.PermissionName,Core.Domain.Roles.RoleType.System),
		new RolePermissionValue(PermissionLanguageUpdate.PermissionName,Core.Domain.Roles.RoleType.System),
		new RolePermissionValue(PermissionLanguageRemove.PermissionName,Core.Domain.Roles.RoleType.System)];
}
