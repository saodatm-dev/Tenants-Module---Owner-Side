using Core.Infrastructure.Authorization;
using Identity.Application.Core.Abstractions.Data;

namespace Identity.Infrastructure.Authorization;

internal sealed class IdentityPermissionProvider(IIdentityDbContext dbContext) : IPermissionProvider
{
	public ValueTask<List<string>> GetByRoleIdAsync(Guid roleId) =>
		dbContext.GetPermissionNamesByRoleIdAsync(roleId).ToListAsync();
}
