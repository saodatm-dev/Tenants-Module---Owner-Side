using System.ComponentModel.DataAnnotations;
using Core.Application.Abstractions.Messaging;

namespace Building.Application.Moderations.RealEstates.GetById;

public sealed record GetModerationRealEstateByIdQuery([property: Required] Guid Id) : IQuery<GetModerationRealEstateByIdResponse>;
