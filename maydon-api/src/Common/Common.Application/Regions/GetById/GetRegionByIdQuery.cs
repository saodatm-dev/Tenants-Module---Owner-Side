using System.ComponentModel.DataAnnotations;
using Core.Application.Abstractions.Messaging;

namespace Common.Application.Regions.GetById;

public sealed record GetRegionByIdQuery([property: Required] Guid Id) : IQuery<GetRegionByIdResponse>;
