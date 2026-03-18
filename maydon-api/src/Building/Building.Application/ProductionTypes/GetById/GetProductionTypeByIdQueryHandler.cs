using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Domain.Languages;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.ProductionTypes.GetById;

internal sealed class GetProductionTypeByIdQueryHandler(IBuildingDbContext dbContext) : IQueryHandler<GetProductionTypeByIdQuery, GetProductionTypeByIdResponse>
{
	public async Task<Result<GetProductionTypeByIdResponse>> Handle(GetProductionTypeByIdQuery request, CancellationToken cancellationToken)
	{
		return await dbContext.ProductionTypeTranslates
			.AsNoTracking()
			.Where(item => item.ProductionTypeId == request.Id)
			.GroupBy(item => item.ProductionTypeId)
			.Select(item => new GetProductionTypeByIdResponse(
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
