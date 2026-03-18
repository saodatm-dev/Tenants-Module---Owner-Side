using System.ComponentModel.DataAnnotations;
using Core.Application.Abstractions.Messaging;

namespace Identity.Application.Roles.GetById;

public sealed record GetRoleByIdQuery([property: Required] Guid Id) : IQuery<GetRoleByIdResponse>;
