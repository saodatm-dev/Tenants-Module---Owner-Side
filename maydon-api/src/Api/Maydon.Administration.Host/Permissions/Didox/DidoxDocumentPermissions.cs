using Core.Domain.Permissions;
using Core.Domain.RolePermissions;
using Maydon.Administration.Host.Abstractions;

namespace Maydon.Administration.Host.Permissions.Didox;

internal sealed class DidoxDocumentPermissions : IPermission
{
    internal static string GroupName => "didox";

    internal static PermissionType PermissionDidoxDocumentGetHtml = new PermissionType("didox-documents:get-html");
    internal static PermissionType PermissionDidoxDocumentGetPdf = new PermissionType("didox-documents:get-pdf");
    internal static PermissionType PermissionDidoxDocumentGetJson = new PermissionType("didox-documents:get-json");

    internal static IEnumerable<RolePermissionValue> DefaultRolePermissions = [
        new RolePermissionValue(PermissionDidoxDocumentGetHtml.PermissionName, Core.Domain.Roles.RoleType.System),
        new RolePermissionValue(PermissionDidoxDocumentGetPdf.PermissionName, Core.Domain.Roles.RoleType.System),
        new RolePermissionValue(PermissionDidoxDocumentGetJson.PermissionName, Core.Domain.Roles.RoleType.System)];
}
