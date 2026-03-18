using Core.Application.Abstractions.Messaging;

namespace Building.Application.MeterTariffs.GetById;

public sealed record GetMeterTariffByIdQuery(Guid Id) : IQuery<GetMeterTariffByIdResponse>;
