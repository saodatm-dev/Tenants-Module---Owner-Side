using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Pagination;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Renovations.Get;

internal sealed class GetRenovationsQueryHandler(IBuildingDbContext dbContext) : IQueryHandler<GetRenovationsQuery, PagedList<GetRenovationsResponse>>
{
	public async Task<Result<PagedList<GetRenovationsResponse>>> Handle(GetRenovationsQuery request, CancellationToken cancellationToken)
	{
		var query = dbContext.RenovationTranslates
			.Where(item => !string.IsNullOrWhiteSpace(request.Filter) ? EF.Functions.Like(item.Value.ToLower(), $"%{request.Filter.ToLowerInvariant()}%") : true)
			.Select(item => new GetRenovationsResponse(
				item.RenovationId,
				item.Value))
			.AsNoTracking();

		int totalCount = await query.CountAsync(cancellationToken);

		var responsesPage = await query
			.Skip(request.Page)
			.Take(request.PageSize)
			.ToListAsync(cancellationToken);

		return new PagedList<GetRenovationsResponse>(responsesPage, request.Page, request.PageSize, totalCount);
	}
}
