using System.ComponentModel.DataAnnotations;
using Core.Application.Abstractions.Messaging;

namespace Identity.Application.Invitations.GetByKey;

public sealed record GetInvitationByKeyQuery([property: Required] string Key) : IQuery<GetInvitationByKeyResponse>;
