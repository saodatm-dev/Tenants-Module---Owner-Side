using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Domain.Results;
using Identity.Application.Core.Abstractions.Data;

namespace Identity.Application.Users.GetPermissions;

internal sealed class GetPermissionsQueryHandler(
	IExecutionContextProvider executionContextProvider,
	IIdentityDbContext dbContext) : IQueryHandler<GetPermissionsQuery, IEnumerable<GetPermissionsResponse>>
{
	public async Task<Result<IEnumerable<GetPermissionsResponse>>> Handle(GetPermissionsQuery request, CancellationToken cancellationToken)
	{
		if (executionContextProvider.IsIndividual)
			return Result<IEnumerable<GetPermissionsResponse>>.None;

		return await dbContext.GetPermissionsByRoleIdAsync(executionContextProvider.RoleId);
	}
}
