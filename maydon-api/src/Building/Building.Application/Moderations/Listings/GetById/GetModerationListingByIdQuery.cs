using System.ComponentModel.DataAnnotations;
using Core.Application.Abstractions.Messaging;

namespace Building.Application.Moderations.Listings.GetById;

public sealed record GetModerationListingByIdQuery([property: Required] Guid Id) : IQuery<GetModerationListingByIdResponse>;
