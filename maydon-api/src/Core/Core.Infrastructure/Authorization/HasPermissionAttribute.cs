using Core.Domain.RolePermissions;
using Microsoft.AspNetCore.Authorization;

namespace Core.Infrastructure.Authorization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public sealed class HasPermissionAttribute(string permissionName, params RolePermissionValue[] rolePermissinValues) : AuthorizeAttribute(permissionName)
{
	public string PermissionName => permissionName;

	public RolePermissionValue[] RoleTypes = rolePermissinValues;
}
