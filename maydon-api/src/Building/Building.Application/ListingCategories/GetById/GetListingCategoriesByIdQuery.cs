using System.ComponentModel.DataAnnotations;
using Core.Application.Abstractions.Messaging;

namespace Building.Application.ListingCategories.GetById;

public sealed record GetListingCategoriesByIdQuery([property: Required] Guid Id) : IQuery<GetListingCategoriesByIdResponse>;
