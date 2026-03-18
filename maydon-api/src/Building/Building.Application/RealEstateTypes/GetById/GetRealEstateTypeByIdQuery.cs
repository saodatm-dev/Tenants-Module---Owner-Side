using System.ComponentModel.DataAnnotations;
using Core.Application.Abstractions.Messaging;

namespace Building.Application.RealEstateTypes.GetById;

public sealed record GetRealEstateTypeByIdQuery([property: Required] Guid Id) : IQuery<GetRealEstateTypeByIdResponse>;
