using Common.Domain;
using Core.Domain.Permissions;
using Core.Domain.RolePermissions;
using Maydon.Administration.Host.Abstractions;

namespace Maydon.Administration.Host.Permissions.Common;

internal sealed class RegionPermissions : IPermission
{
	internal static string GroupName => AssemblyReference.Instance;
	internal static PermissionType PermissionRegionCreate = new PermissionType("regions:create");
	internal static PermissionType PermissionRegionUpdate = new PermissionType("regions:update");
	internal static PermissionType PermissionRegionRemove = new PermissionType("regions:remove");


	internal static IEnumerable<RolePermissionValue> DefaultRolePermissions = [
		new RolePermissionValue(PermissionRegionCreate.PermissionName,Core.Domain.Roles.RoleType.System),
		new RolePermissionValue(PermissionRegionUpdate.PermissionName,Core.Domain.Roles.RoleType.System),
		new RolePermissionValue(PermissionRegionRemove.PermissionName,Core.Domain.Roles.RoleType.System)];
}
