using Common.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Pagination;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Common.Application.Regions.Get;

internal sealed class GetRegionsQueryHandler(
	ICommonDbContext dbContext) : IQueryHandler<GetRegionsQuery, PagedList<GetRegionsResponse>>
{
	public async Task<Result<PagedList<GetRegionsResponse>>> Handle(GetRegionsQuery request, CancellationToken cancellationToken)
	{
		var query = dbContext.RegionTranslates
			.Include(item => item.Region)
			.Where(item => !string.IsNullOrWhiteSpace(request.Filter) ? EF.Functions.Like(item.Value.ToLower(), $"%{request.Filter.ToLowerInvariant()}%") : true)
			.OrderBy(item => item.Region.Order)
			.Select(item => new GetRegionsResponse(
				item.RegionId,
				item.Value,
				item.Region.Order))
			.AsNoTracking();

		int totalCount = await query.CountAsync(cancellationToken);

		var responsesPage = await query
			.Skip(request.Page)
			.Take(request.PageSize)
			.ToListAsync(cancellationToken);

		return new PagedList<GetRegionsResponse>(responsesPage, request.Page, request.PageSize, totalCount);
	}
}
