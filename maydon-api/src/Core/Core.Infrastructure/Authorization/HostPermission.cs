using Core.Domain.Roles;

namespace Core.Infrastructure.Authorization;

public sealed class HostPermission(
	string permissionName,
	bool defaultValue = true)
{
	public string PermissionName => permissionName;
	public bool IsDefault => defaultValue;
	public RoleType RoleType => RoleType.Host;
}
