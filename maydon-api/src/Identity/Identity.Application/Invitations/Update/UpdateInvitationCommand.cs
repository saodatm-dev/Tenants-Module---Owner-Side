using Core.Application.Abstractions.Messaging;

namespace Identity.Application.Invitations.Update;

public sealed record UpdateInvitationCommand(Guid Id, string PhoneNumber) : ICommand<Guid>;
