using Core.Domain.Permissions;
using Core.Domain.RolePermissions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Core.Application.Abstractions.Data;

public interface IApplicationDbContext : IAsyncDisposable
{
	const string
		IsDeletedFilter = nameof(IsDeletedFilter),
		IsInstanceFilter = nameof(IsInstanceFilter),
		IsActiveFilter = nameof(IsActiveFilter),
		TenantIdFilter = nameof(TenantIdFilter),
		TranslateFilter = nameof(TranslateFilter);
	DatabaseFacade Database { get; }
	DbSet<Permission> Permissions { get; }
	DbSet<PermissionTranslate> PermissionTranslates { get; }
	DbSet<RolePermission> RolePermissions { get; }
	Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
	IAsyncEnumerable<string> GetPermissionNamesByRoleIdAsync(Guid roleId);
}
