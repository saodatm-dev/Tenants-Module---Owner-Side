using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.MeterTariffs.GetById;

internal sealed class GetMeterTariffByIdQueryHandler(IBuildingDbContext dbContext) : IQueryHandler<GetMeterTariffByIdQuery, GetMeterTariffByIdResponse>
{
	public async Task<Result<GetMeterTariffByIdResponse>> Handle(GetMeterTariffByIdQuery request, CancellationToken cancellationToken) =>
		await (from meterTariff in dbContext.MeterTariffs
				.AsNoTracking()
				.Where(item => item.Id == request.Id)

			   let meterTypeName = dbContext.MeterTypeTranslates
					.Where(item =>
						item.MeterTypeId == meterTariff.MeterTypeId &&
						item.Field == Domain.MeterTypes.MeterTypeField.Name)
					.Select(item => item.Value)
					.FirstOrDefault()

			   select new GetMeterTariffByIdResponse(
				   meterTariff.Id,
				   meterTariff.MeterTypeId,
				   meterTypeName,
				   meterTariff.Price,
				   meterTariff.Type,
				   meterTariff.IsActual,
				   meterTariff.MinLimit,
				   meterTariff.MaxLimit,
				   meterTariff.FixedPrice,
				   meterTariff.Season,
				   meterTariff.SocialNormLimit))
				.FirstOrDefaultAsync(cancellationToken);
}
