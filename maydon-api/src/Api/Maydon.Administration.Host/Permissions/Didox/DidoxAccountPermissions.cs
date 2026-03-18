using Core.Domain.Permissions;
using Core.Domain.RolePermissions;
using Maydon.Administration.Host.Abstractions;

namespace Maydon.Administration.Host.Permissions.Didox;

internal sealed class DidoxAccountPermissions : IPermission
{
    internal static string GroupName => "didox";

    internal static PermissionType PermissionDidoxAccountList = new PermissionType("didox-accounts:list");
    internal static PermissionType PermissionDidoxAccountGetById = new PermissionType("didox-accounts:get-by-id");
    internal static PermissionType PermissionDidoxAccountCreate = new PermissionType("didox-accounts:create");
    internal static PermissionType PermissionDidoxAccountUpdate = new PermissionType("didox-accounts:update");
    internal static PermissionType PermissionDidoxAccountDelete = new PermissionType("didox-accounts:delete");

    internal static IEnumerable<RolePermissionValue> DefaultRolePermissions = [
        new RolePermissionValue(PermissionDidoxAccountList.PermissionName, Core.Domain.Roles.RoleType.System),
        new RolePermissionValue(PermissionDidoxAccountGetById.PermissionName, Core.Domain.Roles.RoleType.System),
        new RolePermissionValue(PermissionDidoxAccountCreate.PermissionName, Core.Domain.Roles.RoleType.System),
        new RolePermissionValue(PermissionDidoxAccountUpdate.PermissionName, Core.Domain.Roles.RoleType.System),
        new RolePermissionValue(PermissionDidoxAccountDelete.PermissionName, Core.Domain.Roles.RoleType.System)];
}
