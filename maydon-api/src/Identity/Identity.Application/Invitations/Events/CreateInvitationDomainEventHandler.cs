using Core.Domain.Events;
using Identity.Application.Core.Abstractions.Services.Otp;
using Identity.Domain.Invitations.Events;

namespace Identity.Application.Invitations.Events;

internal sealed class CreateInvitationDomainEventHandler(
	IOtpService otpService) : IDomainEventHandler<CreateInvitationDomainEvent>
{
	public async ValueTask Handle(CreateInvitationDomainEvent @event, CancellationToken cancellationToken)
	{
		if (@event.Invitation.RecipientId is Guid recipientId && recipientId != Guid.Empty)
		{
			// send by web socket
		}
		else if (!string.IsNullOrWhiteSpace(@event.Invitation.ReceipientPhoneNumber))
			await otpService.SendAsync(@event.Invitation.ReceipientPhoneNumber!, @event.Invitation.Content, cancellationToken);
	
		await ValueTask.CompletedTask;
	}
}
