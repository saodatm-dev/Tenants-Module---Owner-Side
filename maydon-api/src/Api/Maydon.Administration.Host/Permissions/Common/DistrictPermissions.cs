using Common.Domain;
using Core.Domain.Permissions;
using Core.Domain.RolePermissions;
using Maydon.Administration.Host.Abstractions;

namespace Maydon.Administration.Host.Permissions.Common;

internal sealed class DistrictPermissions : IPermission
{
	internal static string GroupName => AssemblyReference.Instance;
	internal static PermissionType PermissionDistrictCreate = new PermissionType("districts:create");
	internal static PermissionType PermissionDistrictUpdate = new PermissionType("districts:update");
	internal static PermissionType PermissionDistrictRemove = new PermissionType("districts:remove");


	internal static IEnumerable<RolePermissionValue> DefaultRolePermissions = [
		new RolePermissionValue(PermissionDistrictCreate.PermissionName,Core.Domain.Roles.RoleType.System),
		new RolePermissionValue(PermissionDistrictUpdate.PermissionName,Core.Domain.Roles.RoleType.System),
		new RolePermissionValue(PermissionDistrictRemove.PermissionName,Core.Domain.Roles.RoleType.System)];
}
