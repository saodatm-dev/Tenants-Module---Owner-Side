using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Domain.Languages;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Categories.GetById;

internal sealed class GetCategoryByIdQueryHandler(IBuildingDbContext dbContext) : IQueryHandler<GetCategoryByIdQuery, GetCategoryByIdResponse>
{
	public async Task<Result<GetCategoryByIdResponse>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
	{
		return await dbContext.CategoryTranslates
			.AsNoTracking()
			.Where(item => item.CategoryId == request.Id)
			.GroupBy(item => item.CategoryId)
			.Select(item => new GetCategoryByIdResponse(
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
