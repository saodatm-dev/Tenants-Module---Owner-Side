using Core.Application.Abstractions.Messaging;

namespace Building.Application.MeterTypes.Get;

public sealed record GetMeterTypesQuery : IQuery<IEnumerable<GetMeterTypesResponse>>;
