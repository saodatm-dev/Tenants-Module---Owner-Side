using Core.Application.Abstractions.Messaging;
using Core.Application.Pagination;
using Core.Domain.Results;
using Identity.Application.Core.Abstractions.Data;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Roles.Get;

internal sealed class GetRolesQueryHandler(
	IIdentityDbContext dbContext) : IQueryHandler<GetRolesQuery, PagedList<GetRolesResponse>>
{
	public async Task<Result<PagedList<GetRolesResponse>>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
	{
		var query = dbContext.Roles
			.Where(item => !string.IsNullOrWhiteSpace(request.Filter)
				? EF.Functions.Like(item.Name.ToLower(), $"%{request.Filter.ToLowerInvariant()}%")
				: true)
			.OrderByDescending(item => item.UpdatedAt)
			.ThenByDescending(item => item.CreatedAt)
			.Select(item => new GetRolesResponse(
				item.Id,
				item.Name,
				item.TenantId == null))
			.AsNoTracking();

		int totalCount = await query.CountAsync(cancellationToken);

		var responsesPage = await query
			.Skip(request.Page)
			.Take(request.PageSize)
			.ToListAsync(cancellationToken);

		return new PagedList<GetRolesResponse>(responsesPage, request.Page, request.PageSize, totalCount);
	}
}
