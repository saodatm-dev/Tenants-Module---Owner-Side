using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Domain.Languages;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.MeterTypes.GetById;

internal sealed class GetMeterTypeByIdQueryHandler(IBuildingDbContext dbContext) : IQueryHandler<GetMeterTypeByIdQuery, GetMeterTypeByIdResponse>
{
	public async Task<Result<GetMeterTypeByIdResponse>> Handle(GetMeterTypeByIdQuery request, CancellationToken cancellationToken) =>
		await (from meterType in dbContext.MeterTypes
				.AsNoTracking()
				.IgnoreQueryFilters([IApplicationDbContext.TranslateFilter])
				.Where(item => item.Id == request.Id)

			   let translates = dbContext.MeterTypeTranslates.Where(item => item.MeterTypeId == meterType.Id)

			   select new GetMeterTypeByIdResponse(
				   meterType.Id,
				   translates.Where(item => item.Field == Domain.MeterTypes.MeterTypeField.Name)
					   .Select(item => new LanguageValue(item.LanguageId, item.LanguageShortCode, item.Value)),
				   translates.Where(item => item.Field == Domain.MeterTypes.MeterTypeField.Description)
					   .Select(item => new LanguageValue(item.LanguageId, item.LanguageShortCode, item.Value)),
				   meterType.Icon))
				.FirstOrDefaultAsync(cancellationToken);
}
