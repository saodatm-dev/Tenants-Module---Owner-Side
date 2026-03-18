using Core.Domain.Permissions;
using Core.Domain.RolePermissions;
using Maydon.Administration.Host.Abstractions;

namespace Maydon.Administration.Host.Permissions.Document;

internal sealed class ContractPermissions : IPermission
{
    internal static string GroupName => "documents";

    internal static PermissionType PermissionContractList = new PermissionType("contracts:list");
    internal static PermissionType PermissionContractGetById = new PermissionType("contracts:get-by-id");
    internal static PermissionType PermissionContractCreate = new PermissionType("contracts:create");
    internal static PermissionType PermissionContractUpdateBody = new PermissionType("contracts:update-body");
    internal static PermissionType PermissionContractRegenerate = new PermissionType("contracts:regenerate");
    internal static PermissionType PermissionContractReject = new PermissionType("contracts:reject");
    internal static PermissionType PermissionContractExportDidox = new PermissionType("contracts:export-didox");
    internal static PermissionType PermissionContractSyncDidox = new PermissionType("contracts:sync-didox");
    internal static PermissionType PermissionContractUploadAttachment = new PermissionType("contracts:upload-attachment");

    internal static IEnumerable<RolePermissionValue> DefaultRolePermissions = [
        new RolePermissionValue(PermissionContractList.PermissionName, Core.Domain.Roles.RoleType.System),
        new RolePermissionValue(PermissionContractGetById.PermissionName, Core.Domain.Roles.RoleType.System),
        new RolePermissionValue(PermissionContractCreate.PermissionName, Core.Domain.Roles.RoleType.System),
        new RolePermissionValue(PermissionContractUpdateBody.PermissionName, Core.Domain.Roles.RoleType.System),
        new RolePermissionValue(PermissionContractRegenerate.PermissionName, Core.Domain.Roles.RoleType.System),
        new RolePermissionValue(PermissionContractReject.PermissionName, Core.Domain.Roles.RoleType.System),
        new RolePermissionValue(PermissionContractExportDidox.PermissionName, Core.Domain.Roles.RoleType.System),
        new RolePermissionValue(PermissionContractSyncDidox.PermissionName, Core.Domain.Roles.RoleType.System),
        new RolePermissionValue(PermissionContractUploadAttachment.PermissionName, Core.Domain.Roles.RoleType.System)];
}
