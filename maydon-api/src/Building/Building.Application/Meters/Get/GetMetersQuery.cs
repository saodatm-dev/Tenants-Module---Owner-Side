using Core.Application.Abstractions.Messaging;

namespace Building.Application.Meters.Get;

public sealed record GetMetersQuery(Guid RealEstateId) : IQuery<IEnumerable<GetMetersResponse>>;
