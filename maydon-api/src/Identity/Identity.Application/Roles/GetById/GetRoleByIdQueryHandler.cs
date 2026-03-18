using Core.Application.Abstractions.Messaging;
using Core.Domain.Results;
using Identity.Application.Core.Abstractions.Data;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Roles.GetById;

internal sealed class GetRoleByIdQueryHandler(IIdentityDbContext dbContext) : IQueryHandler<GetRoleByIdQuery, GetRoleByIdResponse>
{
	public async Task<Result<GetRoleByIdResponse>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
	{
		var role = await dbContext.Roles.AsNoTracking().FirstOrDefaultAsync(item => item.Id == request.Id, cancellationToken);

		if (role is null)
			return Result<GetRoleByIdResponse>.None;

		var permissions = await dbContext.GetPermissionsByRoleIdAsync(role.Id);

		return new GetRoleByIdResponse(
			role.Id,
			role.Name,
			permissions.Select(item =>
				new GetRoleByIdPermissionsResponse(item.Id, item.Name, item.IsActive)));
	}
}
