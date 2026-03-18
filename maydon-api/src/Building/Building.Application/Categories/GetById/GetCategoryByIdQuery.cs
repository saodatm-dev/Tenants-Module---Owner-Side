using System.ComponentModel.DataAnnotations;
using Core.Application.Abstractions.Messaging;

namespace Building.Application.Categories.GetById;

public sealed record GetCategoryByIdQuery([property: Required] Guid Id) : IQuery<GetCategoryByIdResponse>;
