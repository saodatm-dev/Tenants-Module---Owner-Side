using System.ComponentModel.DataAnnotations;
using Core.Application.Abstractions.Messaging;

namespace Building.Application.LandCategories.GetById;

public sealed record GetLandCategoryByIdQuery([property: Required] Guid Id) : IQuery<GetLandCategoryByIdResponse>;
