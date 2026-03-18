using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Domain.Languages;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.LandCategories.GetById;

internal sealed class GetLandCategoryByIdQueryHandler(IBuildingDbContext dbContext) : IQueryHandler<GetLandCategoryByIdQuery, GetLandCategoryByIdResponse>
{
	public async Task<Result<GetLandCategoryByIdResponse>> Handle(GetLandCategoryByIdQuery request, CancellationToken cancellationToken)
	{
		return await dbContext.LandCategoryTranslates
			.AsNoTracking()
			.Where(item => item.LandCategoryId == request.Id)
			.GroupBy(item => item.LandCategoryId)
			.Select(item => new GetLandCategoryByIdResponse(
				item.Key,
				item.ToList()
					.Select(translate =>
						new LanguageValue(
							translate.LanguageId,
							translate.LanguageShortCode,
							translate.Value))))
			.FirstOrDefaultAsync(cancellationToken);
	}
}
