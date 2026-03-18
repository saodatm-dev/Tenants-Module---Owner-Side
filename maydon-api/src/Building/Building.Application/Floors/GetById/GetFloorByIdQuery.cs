using System.ComponentModel.DataAnnotations;
using Core.Application.Abstractions.Messaging;

namespace Building.Application.Floors.GetById;

public sealed record GetFloorByIdQuery([property: Required] Guid Id) : IQuery<GetFloorByIdResponse>;
