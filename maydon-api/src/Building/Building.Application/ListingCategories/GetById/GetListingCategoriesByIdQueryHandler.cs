using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Domain.Languages;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.ListingCategories.GetById;

internal sealed class GetListingCategoriesByIdQueryHandler(IBuildingDbContext dbContext) : IQueryHandler<GetListingCategoriesByIdQuery, GetListingCategoriesByIdResponse>
{
	public async Task<Result<GetListingCategoriesByIdResponse>> Handle(GetListingCategoriesByIdQuery request, CancellationToken cancellationToken)
	{
		return await dbContext.ListingCategoryTranslates
			.Where(item => item.ListingCategoryId == request.Id)
			.Include(item => item.ListingCategory)
			.GroupBy(item => item.ListingCategoryId)
			.Select(item => new GetListingCategoriesByIdResponse(
				item.Key,
				item.First().ListingCategory.ParentId,
				item.First().ListingCategory.BuildingType,
				item.First().ListingCategory.IconUrl,
				item.ToList()
					.Select(translate =>
						new LanguageValue(
							translate.LanguageId,
							translate.LanguageShortCode,
							translate.Value))))
			.FirstOrDefaultAsync(cancellationToken);
	}
}
