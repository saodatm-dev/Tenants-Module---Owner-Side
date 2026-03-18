using Core.Domain.Events;
using Identity.Application.Core.Abstractions.Data;
using Identity.Domain.Invitations;
using Identity.Domain.Users.Events;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Users.Events;

internal sealed class UpsertUserPostDomainEventHandler(IIdentityDbContext dbContext) : IDomainEventHandler<UpsertUserPostDomainEvent>
{
	public async ValueTask Handle(UpsertUserPostDomainEvent @event, CancellationToken cancellationToken)
	{
		if (string.IsNullOrWhiteSpace(@event.User.PhoneNumber))
			return;

		var userPhone = @event.User.PhoneNumber.Trim();
		var normalizedPhone = userPhone.TrimStart('+');
		var phoneWithPlus = userPhone.StartsWith('+') ? userPhone : $"+{userPhone}";

		var pendingInvitations = await dbContext.Invitations
			.IgnoreQueryFilters()
			.Where(i =>
				!i.IsDeleted &&
				i.RecipientId == null &&
				i.Status == InvitationStatus.Sent &&
				i.ReceipientPhoneNumber != null &&
				(i.ReceipientPhoneNumber == userPhone ||
				 i.ReceipientPhoneNumber == normalizedPhone ||
				 i.ReceipientPhoneNumber == phoneWithPlus ||
				 i.ReceipientPhoneNumber.Replace("+", "") == normalizedPhone))
			.ToListAsync(cancellationToken);

		if (pendingInvitations.Count == 0)
			return;

		// Link each invitation to the newly registered user
		foreach (var invitation in pendingInvitations)
		{
			invitation.LinkToUser(@event.User.Id);
		}

		dbContext.Invitations.UpdateRange(pendingInvitations);
	}
}
