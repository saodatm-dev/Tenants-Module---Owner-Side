using System.ComponentModel.DataAnnotations;
using Core.Application.Abstractions.Messaging;

namespace Building.Application.AmenityCategories.GetById;

public sealed record GetAmenityCategoryByIdQuery([property: Required] Guid Id) : IQuery<GetAmenityCategoryByIdResponse>;
