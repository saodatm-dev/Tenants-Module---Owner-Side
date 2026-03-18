using Common.Application.Core.Abstractions.Data;
using Core.Infrastructure.Authorization;

namespace Common.Infrastructure.Authorization;

internal sealed class CommonPermissionProvider(ICommonDbContext dbContext) : IPermissionProvider
{
	public ValueTask<List<string>> GetByRoleIdAsync(Guid roleId) =>
		dbContext.GetPermissionNamesByRoleIdAsync(roleId).ToListAsync();
}
