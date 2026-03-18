using System.ComponentModel.DataAnnotations;
using Core.Application.Abstractions.Messaging;

namespace Building.Application.RealEstates.GetById;

public sealed record GetRealEstateByIdQuery([property: Required] Guid Id) : IQuery<GetRealEstateByIdResponse>;
