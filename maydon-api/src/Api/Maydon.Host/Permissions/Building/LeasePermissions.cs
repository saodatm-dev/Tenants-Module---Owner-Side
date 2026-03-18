using Building.Domain;
using Core.Domain.Permissions;
using Core.Domain.RolePermissions;
using Maydon.Host.Abstractions;

namespace Maydon.Host.Permissions.Building;

internal sealed class LeasePermissions : IPermission
{
    internal static string GroupName => AssemblyReference.Instance;

    internal static PermissionType PermissionLeaseCreate = new PermissionType("leases:create");
    internal static PermissionType PermissionLeaseUpdate = new PermissionType("leases:update");
    internal static PermissionType PermissionLeaseRemove = new PermissionType("leases:remove");

    internal static IEnumerable<RolePermissionValue> DefaultRolePermissions = [
        new RolePermissionValue(PermissionLeaseCreate.PermissionName, Core.Domain.Roles.RoleType.System),
        new RolePermissionValue(PermissionLeaseUpdate.PermissionName, Core.Domain.Roles.RoleType.System),
        new RolePermissionValue(PermissionLeaseRemove.PermissionName, Core.Domain.Roles.RoleType.System),
        new RolePermissionValue(PermissionLeaseCreate.PermissionName, Core.Domain.Roles.RoleType.Owner),
        new RolePermissionValue(PermissionLeaseUpdate.PermissionName, Core.Domain.Roles.RoleType.Owner),
        new RolePermissionValue(PermissionLeaseRemove.PermissionName, Core.Domain.Roles.RoleType.Owner)];
}
