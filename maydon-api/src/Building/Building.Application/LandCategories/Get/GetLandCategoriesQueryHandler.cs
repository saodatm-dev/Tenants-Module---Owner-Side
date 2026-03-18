using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Pagination;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.LandCategories.Get;

internal sealed class GetLandCategoriesQueryHandler(IBuildingDbContext dbContext) : IQueryHandler<GetLandCategoriesQuery, PagedList<GetLandCategoriesResponse>>
{
	public async Task<Result<PagedList<GetLandCategoriesResponse>>> Handle(GetLandCategoriesQuery request, CancellationToken cancellationToken)
	{
		var query = dbContext.LandCategoryTranslates
			.Where(item => !string.IsNullOrWhiteSpace(request.Filter) ? EF.Functions.Like(item.Value.ToLower(), $"%{request.Filter.ToLowerInvariant()}%") : true)
			.Select(item => new GetLandCategoriesResponse(
				item.LandCategoryId,
				item.Value))
			.AsNoTracking();

		int totalCount = await query.CountAsync(cancellationToken);

		var responsesPage = await query
			.Skip(request.Page)
			.Take(request.PageSize)
			.ToListAsync(cancellationToken);

		return new PagedList<GetLandCategoriesResponse>(responsesPage, request.Page, request.PageSize, totalCount);
	}
}
