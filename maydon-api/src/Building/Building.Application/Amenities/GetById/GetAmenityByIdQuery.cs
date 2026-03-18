using System.ComponentModel.DataAnnotations;
using Core.Application.Abstractions.Messaging;

namespace Building.Application.Amenities.GetById;

public sealed record GetAmenityByIdQuery([property: Required] Guid Id) : IQuery<GetAmenityByIdResponse>;
