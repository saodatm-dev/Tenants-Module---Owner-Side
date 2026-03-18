namespace Core.Domain.Permissions;

public sealed record PermissionType(string PermissionName, bool IsSystem = false);
