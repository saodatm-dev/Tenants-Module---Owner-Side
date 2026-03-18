namespace Core.Infrastructure.Authorization;

public interface IPermissionProvider
{
	public ValueTask<List<string>> GetByRoleIdAsync(Guid roleId);
}
