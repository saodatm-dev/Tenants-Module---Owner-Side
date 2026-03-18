using Core.Application.Abstractions.Messaging;

namespace Building.Application.RealEstateTypes.Get;

public sealed record GetRealEstateTypesQuery(string? Filter = null) : IQuery<IEnumerable<GetRealEstateTypesResponse>>;
