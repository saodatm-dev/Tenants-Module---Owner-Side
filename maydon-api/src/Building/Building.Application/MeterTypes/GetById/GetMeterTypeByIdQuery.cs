using Core.Application.Abstractions.Messaging;

namespace Building.Application.MeterTypes.GetById;

public sealed record GetMeterTypeByIdQuery(Guid Id) : IQuery<GetMeterTypeByIdResponse>;
