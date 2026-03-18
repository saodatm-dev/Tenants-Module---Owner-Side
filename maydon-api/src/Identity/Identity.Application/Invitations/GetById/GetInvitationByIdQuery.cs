using System.ComponentModel.DataAnnotations;
using Core.Application.Abstractions.Messaging;

namespace Identity.Application.Invitations.GetById;

public sealed record GetInvitationByIdQuery([property: Required] Guid Id) : IQuery<GetInvitationByIdResponse>;
