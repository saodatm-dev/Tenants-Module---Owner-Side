using Core.Domain.Roles;

namespace Core.Domain.RolePermissions;

public sealed record RolePermissionValue(string PermissionName, RoleType Type, bool Value = true);
