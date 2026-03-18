using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Identity.Application.Core.Abstractions.Data;

using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Invitations.Reject;

internal sealed class RejectInvitationCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IExecutionContextProvider executionContextProvider,
	IIdentityDbContext dbContext) : ICommandHandler<RejectInvitationCommand>
{
	public async Task<Result> Handle(RejectInvitationCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.Invitations
			.FirstOrDefaultAsync(item =>
				item.RecipientId == executionContextProvider.UserId &&
				item.Status == Domain.Invitations.InvitationStatus.Sent, cancellationToken);

		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.InvitationNotFound(nameof(RejectInvitationCommand.Id)));

		dbContext.Invitations.Update(maybeItem.Reject(command.Reason));

		await dbContext.SaveChangesAsync(cancellationToken);

		return Result.Success();
	}
}
