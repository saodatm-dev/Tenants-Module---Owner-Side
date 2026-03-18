
using Building.Domain.MeterTariffs;

namespace Building.Application.MeterTariffs.GetById;

public sealed record GetMeterTariffByIdResponse(
	Guid Id,
	Guid MeterTypeId,
	string MeterType,
	long Price,
	MeterTariffType Type,
	bool IsActual,
	decimal? MinLimit = null,
	decimal? MaxLimit = null,
	long? FixedPrice = null,
	Season Season = Season.All,
	decimal? SocialNormLimit = null);
