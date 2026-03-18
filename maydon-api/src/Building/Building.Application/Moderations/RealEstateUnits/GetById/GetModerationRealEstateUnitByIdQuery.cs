using System.ComponentModel.DataAnnotations;
using Core.Application.Abstractions.Messaging;

namespace Building.Application.Moderations.RealEstateUnits.GetById;

public sealed record GetModerationRealEstateUnitByIdQuery([property: Required] Guid Id) : IQuery<GetModerationRealEstateUnitByIdResponse>;
