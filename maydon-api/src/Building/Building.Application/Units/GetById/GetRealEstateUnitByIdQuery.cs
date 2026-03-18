using Core.Application.Abstractions.Messaging;

namespace Building.Application.Units.GetById;

public sealed record GetRealEstateUnitByIdQuery(Guid Id) : IQuery<GetRealEstateUnitByIdResponse>;
