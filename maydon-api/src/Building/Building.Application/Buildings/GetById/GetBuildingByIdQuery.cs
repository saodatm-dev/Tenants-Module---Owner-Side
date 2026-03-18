using System.ComponentModel.DataAnnotations;
using Core.Application.Abstractions.Messaging;

namespace Building.Application.Buildings.GetById;

public sealed record GetBuildingByIdQuery([property: Required] Guid Id) : IQuery<GetBuildingByIdResponse>;
