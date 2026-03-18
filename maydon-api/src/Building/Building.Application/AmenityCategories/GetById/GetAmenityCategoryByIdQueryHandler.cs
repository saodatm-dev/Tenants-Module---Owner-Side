using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Domain.Languages;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.AmenityCategories.GetById;

internal sealed class GetAmenityCategoryByIdQueryHandler(IBuildingDbContext dbContext) : IQueryHandler<GetAmenityCategoryByIdQuery, GetAmenityCategoryByIdResponse>
{
	public async Task<Result<GetAmenityCategoryByIdResponse>> Handle(GetAmenityCategoryByIdQuery request, CancellationToken cancellationToken)
	{
		return await dbContext.AmenityCategories
			.AsNoTracking()
			.IgnoreQueryFilters([IApplicationDbContext.TranslateFilter])
			.Where(item => item.Id == request.Id)
			.Include(item => item.Translates)
			.Select(item => new GetAmenityCategoryByIdResponse(
				item.Id,
				item.Translates.Select(t => new LanguageValue(t.LanguageId, t.LanguageShortCode, t.Value))))
			.FirstOrDefaultAsync(cancellationToken);
	}
}
