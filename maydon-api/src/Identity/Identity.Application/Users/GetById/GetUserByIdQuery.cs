using System.ComponentModel.DataAnnotations;
using Core.Application.Abstractions.Messaging;

namespace Identity.Application.Users.GetById;

public sealed record GetUserByIdQuery([property: Required] Guid Id) : IQuery<GetUserByIdResponse>;
