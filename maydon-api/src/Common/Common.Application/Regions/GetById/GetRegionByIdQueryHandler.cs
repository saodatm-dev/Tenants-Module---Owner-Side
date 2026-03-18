using Common.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Domain.Languages;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Common.Application.Regions.GetById;

internal sealed class GetRegionByIdQueryHandler(ICommonDbContext dbContext) : IQueryHandler<GetRegionByIdQuery, GetRegionByIdResponse>
{
	public async Task<Result<GetRegionByIdResponse>> Handle(GetRegionByIdQuery request, CancellationToken cancellationToken)
	{
		return await dbContext.RegionTranslates
			.Include(item => item.Region)
			.AsNoTracking()
			.Where(item => item.RegionId == request.Id)
			.GroupBy(item => item.RegionId)
			.Select(item => new GetRegionByIdResponse(
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
