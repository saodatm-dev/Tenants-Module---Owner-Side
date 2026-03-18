using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Domain.Languages;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Renovations.GetById;

internal sealed class GetRenovationByIdQueryHandler(IBuildingDbContext dbContext) : IQueryHandler<GetRenovationByIdQuery, GetRenovationByIdResponse>
{
	public async Task<Result<GetRenovationByIdResponse>> Handle(GetRenovationByIdQuery request, CancellationToken cancellationToken)
	{
		return await dbContext.RenovationTranslates
			.AsNoTracking()
			.Where(item => item.RenovationId == request.Id)
			.GroupBy(item => item.RenovationId)
			.Select(item => new GetRenovationByIdResponse(
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
