using Common.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Domain.Languages;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Common.Application.Districts.GetById;

internal sealed class GetDistrictByIdQueryHandler(ICommonDbContext dbContext) : IQueryHandler<GetDistrictByIdQuery, GetDistrictByIdResponse>
{
	public async Task<Result<GetDistrictByIdResponse>> Handle(GetDistrictByIdQuery request, CancellationToken cancellationToken)
	{
		return await dbContext.DistrictTranslates
			.Include(item => item.District)
			.AsNoTracking()
			.Where(item => item.DistrictId == request.Id)
			.GroupBy(item => item.DistrictId)
			.Select(item => new GetDistrictByIdResponse(
				item.Key,
				item.ToList().First().District.RegionId,
				item.ToList()
					.Select(translate =>
						new LanguageValue(
							translate.LanguageId,
							translate.LanguageShortCode,
							translate.Value))))
			.FirstOrDefaultAsync(cancellationToken);
	}
}
