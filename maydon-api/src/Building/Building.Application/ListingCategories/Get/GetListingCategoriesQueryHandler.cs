using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Pagination;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.ListingCategories.Get;

internal sealed class GetListingCategoriesQueryHandler(
	IBuildingDbContext dbContext) : IQueryHandler<GetListingCategoriesQuery, PagedList<GetListingCategoriesResponse>>
{
	public async Task<Result<PagedList<GetListingCategoriesResponse>>> Handle(GetListingCategoriesQuery request, CancellationToken cancellationToken)
	{
		var query = dbContext.ListingCategoryTranslates
			.Where(item => !string.IsNullOrWhiteSpace(request.Filter) ? EF.Functions.Like(item.Value.ToLower(), $"%{request.Filter.ToLowerInvariant()}%") : true)
			.Include(item => item.ListingCategory)
			.OrderBy(item => item.ListingCategory.Order)
			.Select(item => new GetListingCategoriesResponse(
				item.ListingCategoryId,
				item.ListingCategory.ParentId,
				item.ListingCategory.BuildingType,
				item.ListingCategory.IconUrl,
				item.Value));

		int totalCount = await query.CountAsync(cancellationToken);

		var responsesPage = await query
			.Skip(request.Page)
			.Take(request.PageSize)
			.ToListAsync(cancellationToken);

		return new PagedList<GetListingCategoriesResponse>(responsesPage, request.Page, request.PageSize, totalCount);
	}
}
