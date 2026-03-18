using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Pagination;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.ProductionTypes.Get;

internal sealed class GetProductionTypesQueryHandler(IBuildingDbContext dbContext) : IQueryHandler<GetProductionTypesQuery, PagedList<GetProductionTypesResponse>>
{
	public async Task<Result<PagedList<GetProductionTypesResponse>>> Handle(GetProductionTypesQuery request, CancellationToken cancellationToken)
	{
		var query = dbContext.ProductionTypeTranslates
			.Where(item => !string.IsNullOrWhiteSpace(request.Filter) ? EF.Functions.Like(item.Value.ToLower(), $"%{request.Filter.ToLowerInvariant()}%") : true)
			.Select(item => new GetProductionTypesResponse(
				item.ProductionTypeId,
				item.Value))
			.AsNoTracking();

		int totalCount = await query.CountAsync(cancellationToken);

		var responsesPage = await query
			.Skip(request.Page)
			.Take(request.PageSize)
			.ToListAsync(cancellationToken);

		return new PagedList<GetProductionTypesResponse>(responsesPage, request.Page, request.PageSize, totalCount);
	}
}
