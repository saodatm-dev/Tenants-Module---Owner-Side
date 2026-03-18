using Core.Application.Abstractions.Messaging;

namespace Building.Application.MeterReadings.GetById;

public sealed record GetMeterReadingByIdQuery(Guid Id) : IQuery<GetMeterReadingByIdResponse>;
