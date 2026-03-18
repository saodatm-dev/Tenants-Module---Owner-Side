using Core.Application.Abstractions.Messaging;

namespace Building.Application.Meters.GetById;

public sealed record GetMeterByIdQuery(Guid Id) : IQuery<GetMeterByIdResponse>;
