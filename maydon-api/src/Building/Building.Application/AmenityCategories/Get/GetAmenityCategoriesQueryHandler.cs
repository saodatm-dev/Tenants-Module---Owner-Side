using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Pagination;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.AmenityCategories.Get;

internal sealed class GetAmenityCategoriesQueryHandler(
	IBuildingDbContext dbContext) : IQueryHandler<GetAmenityCategoriesQuery, PagedList<GetAmenityCategoriesResponse>>
{
	public async Task<Result<PagedList<GetAmenityCategoriesResponse>>> Handle(GetAmenityCategoriesQuery request, CancellationToken cancellationToken)
	{
		var query = dbContext.AmenityCategoryTranslates
			.Where(item => !string.IsNullOrWhiteSpace(request.Filter) ? EF.Functions.Like(item.Value.ToLower(), $"%{request.Filter.ToLowerInvariant()}%") : true)
			.Include(item => item.AmenityCategory)
			.Select(item => new GetAmenityCategoriesResponse(
				item.AmenityCategoryId,
				item.Value))
			.AsNoTracking();

		int totalCount = await query.CountAsync(cancellationToken);

		var responsesPage = await query
			.Skip(request.Page)
			.Take(request.PageSize)
			.ToListAsync(cancellationToken);

		return new PagedList<GetAmenityCategoriesResponse>(responsesPage, request.Page, request.PageSize, totalCount);
	}
}
