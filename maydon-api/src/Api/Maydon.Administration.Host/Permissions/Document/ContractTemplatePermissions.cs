using Core.Domain.Permissions;
using Core.Domain.RolePermissions;
using Maydon.Administration.Host.Abstractions;

namespace Maydon.Administration.Host.Permissions.Document;

internal sealed class ContractTemplatePermissions : IPermission
{
    internal static string GroupName => "documents";

    internal static PermissionType PermissionContractTemplateList = new PermissionType("contract-templates:list");
    internal static PermissionType PermissionContractTemplateGetById = new PermissionType("contract-templates:get-by-id");
    internal static PermissionType PermissionContractTemplateCreate = new PermissionType("contract-templates:create");
    internal static PermissionType PermissionContractTemplateUpdate = new PermissionType("contract-templates:update");
    internal static PermissionType PermissionContractTemplateUpdateBody = new PermissionType("contract-templates:update-body");
    internal static PermissionType PermissionContractTemplateDelete = new PermissionType("contract-templates:delete");
    internal static PermissionType PermissionContractTemplatePreviewPdf = new PermissionType("contract-templates:preview-pdf");

    internal static IEnumerable<RolePermissionValue> DefaultRolePermissions = [
        new RolePermissionValue(PermissionContractTemplateList.PermissionName, Core.Domain.Roles.RoleType.System),
        new RolePermissionValue(PermissionContractTemplateGetById.PermissionName, Core.Domain.Roles.RoleType.System),
        new RolePermissionValue(PermissionContractTemplateCreate.PermissionName, Core.Domain.Roles.RoleType.System),
        new RolePermissionValue(PermissionContractTemplateUpdate.PermissionName, Core.Domain.Roles.RoleType.System),
        new RolePermissionValue(PermissionContractTemplateUpdateBody.PermissionName, Core.Domain.Roles.RoleType.System),
        new RolePermissionValue(PermissionContractTemplateDelete.PermissionName, Core.Domain.Roles.RoleType.System),
        new RolePermissionValue(PermissionContractTemplatePreviewPdf.PermissionName, Core.Domain.Roles.RoleType.System)];
}
