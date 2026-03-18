using Core.Application.Abstractions.Messaging;

namespace Building.Application.Amenities.GetByRealEstateId;

public sealed record GetAmenitiesByRealEstateIdQuery(Guid RealEstateId) : IQuery<IEnumerable<GetAmenitiesByRealEstateIdResponse>>;
