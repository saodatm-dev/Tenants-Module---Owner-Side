using Core.Domain.Permissions;
using Core.Domain.RolePermissions;
using Maydon.Administration.Host.Abstractions;

namespace Maydon.Administration.Host.Permissions.Didox;

internal sealed class DidoxAuthPermissions : IPermission
{
    internal static string GroupName => "didox";

    internal static PermissionType PermissionDidoxAuthRegister = new PermissionType("didox-auth:register");

    internal static IEnumerable<RolePermissionValue> DefaultRolePermissions = [
        new RolePermissionValue(PermissionDidoxAuthRegister.PermissionName, Core.Domain.Roles.RoleType.System)];
}
