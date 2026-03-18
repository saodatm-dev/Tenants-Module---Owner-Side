using System.ComponentModel.DataAnnotations;
using Core.Application.Abstractions.Messaging;

namespace Building.Application.Listings.GetById;

public sealed record GetListingByIdQuery([property: Required] Guid Id) : IQuery<GetListingByIdResponse>;
