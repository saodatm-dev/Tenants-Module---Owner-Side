using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Identity.Application.Core.Abstractions.Data;

using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Invitations.Update;

internal sealed class UpdateInvitationCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IExecutionContextProvider executionContextProvider,
	IIdentityDbContext dbContext) : ICommandHandler<UpdateInvitationCommand, Guid>
{
	public async Task<Result<Guid>> Handle(UpdateInvitationCommand command, CancellationToken cancellationToken)
	{
		if (executionContextProvider.IsIndividual)
			return Result.Failure<Guid>(sharedViewLocalizer.IndividualUserCanNotInvite(nameof(UpdateInvitationCommand.PhoneNumber)));

		if (!executionContextProvider.IsOwner)
			return Result.Failure<Guid>(sharedViewLocalizer.InvitationNoPermission(nameof(UpdateInvitationCommand.PhoneNumber)));

		var recipientId = await dbContext.Users
			.AsNoTracking()
			.Where(item => item.PhoneNumber == command.PhoneNumber)
			.Select(item => item.Id)
			.FirstOrDefaultAsync(cancellationToken);

		if (recipientId == Guid.Empty)
			return Result.Failure<Guid>(sharedViewLocalizer.UserNotFound(nameof(UpdateInvitationCommand.PhoneNumber)));

		var maybeItem = await dbContext.Invitations
			.FirstOrDefaultAsync(item =>
				item.Id == command.Id &&
				item.SenderId == executionContextProvider.TenantId &&
				item.Status != Domain.Invitations.InvitationStatus.Accepted, cancellationToken);

		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.InvitationNotFound(nameof(UpdateInvitationCommand.Id)));

		dbContext.Invitations.Update(maybeItem);

		await dbContext.SaveChangesAsync(cancellationToken);

		return maybeItem.Id;
	}
}
