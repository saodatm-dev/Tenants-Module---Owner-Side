using System.ComponentModel.DataAnnotations;
using Core.Application.Abstractions.Messaging;

namespace Common.Application.Districts.GetById;

public sealed record GetDistrictByIdQuery([property: Required] Guid Id) : IQuery<GetDistrictByIdResponse>;
