using System.ComponentModel.DataAnnotations;
using Core.Application.Abstractions.Messaging;

namespace Building.Application.Leases.GetById;

public sealed record GetLeaseByIdQuery([property: Required] Guid Id) : IQuery<GetLeaseByIdResponse>;
