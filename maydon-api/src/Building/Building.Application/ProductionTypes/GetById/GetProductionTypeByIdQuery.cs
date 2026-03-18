using Core.Application.Abstractions.Messaging;

namespace Building.Application.ProductionTypes.GetById;

public sealed record GetProductionTypeByIdQuery(Guid Id) : IQuery<GetProductionTypeByIdResponse>;
