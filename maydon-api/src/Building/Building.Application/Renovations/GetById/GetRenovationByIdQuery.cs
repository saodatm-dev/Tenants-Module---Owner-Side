using System.ComponentModel.DataAnnotations;
using Core.Application.Abstractions.Messaging;

namespace Building.Application.Renovations.GetById;

public sealed record GetRenovationByIdQuery([property: Required] Guid Id) : IQuery<GetRenovationByIdResponse>;
