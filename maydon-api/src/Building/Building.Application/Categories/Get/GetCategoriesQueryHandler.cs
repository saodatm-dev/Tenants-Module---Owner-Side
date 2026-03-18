using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Pagination;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Categories.Get;

internal sealed class GetCategoriesQueryHandler(
	IBuildingDbContext dbContext) : IQueryHandler<GetCategoriesQuery, PagedList<GetCategoriesResponse>>
{
	public async Task<Result<PagedList<GetCategoriesResponse>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
	{
		var query = dbContext.CategoryTranslates
			.Where(item => !string.IsNullOrWhiteSpace(request.Filter) ? EF.Functions.Like(item.Value.ToLower(), $"%{request.Filter.ToLowerInvariant()}%") : true)
			.Include(item => item.Category)
			.Select(item => new GetCategoriesResponse(
				item.CategoryId,
				item.Category.BuildingType,
				item.Category.IconUrl,
				item.Value))
			.AsNoTracking();

		int totalCount = await query.CountAsync(cancellationToken);

		var responsesPage = await query
			.Skip(request.Page)
			.Take(request.PageSize)
			.ToListAsync(cancellationToken);

		return new PagedList<GetCategoriesResponse>(responsesPage, request.Page, request.PageSize, totalCount);
	}
}
