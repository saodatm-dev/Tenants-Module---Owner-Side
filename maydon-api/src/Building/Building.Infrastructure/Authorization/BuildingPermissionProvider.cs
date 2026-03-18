using Building.Application.Core.Abstractions.Data;
using Core.Infrastructure.Authorization;

namespace Building.Infrastructure.Authorization;

internal sealed class BuildingPermissionProvider(IBuildingDbContext dbContext) : IPermissionProvider
{
	public ValueTask<List<string>> GetByRoleIdAsync(Guid roleId) =>
		dbContext.GetPermissionNamesByRoleIdAsync(roleId).ToListAsync();
}
