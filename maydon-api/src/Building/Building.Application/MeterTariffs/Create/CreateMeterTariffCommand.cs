using Building.Domain.MeterTariffs;
using Core.Application.Abstractions.Messaging;

namespace Building.Application.MeterTariffs.Create;

public sealed record CreateMeterTariffCommand(
	Guid MeterTypeId,
	DateOnly ValidFrom,
	DateOnly? ValidTo,
	long Price,
	MeterTariffType Type = MeterTariffType.Individual,
	bool IsActual = false,
	decimal? MinLimit = null,
	decimal? MaxLimit = null,
	long? FixedPrice = null,
	Season Season = Season.All,
	decimal? SocialNormLimit = null) : ICommand<Guid>;
