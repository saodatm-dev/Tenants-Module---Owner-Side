using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Identity.Application.Core.Abstractions.Data;

using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Invitations.Cancel;

internal sealed class CancelInvitationCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IExecutionContextProvider executionContextProvider,
	IIdentityDbContext dbContext) : ICommandHandler<CancelInvitationCommand>
{
	public async Task<Result> Handle(CancelInvitationCommand command, CancellationToken cancellationToken)
	{
		if (executionContextProvider.IsIndividual)
			return Result.Failure<Guid>(sharedViewLocalizer.InvitationNoPermission(nameof(CancelInvitationCommand.Id)));

		if (!executionContextProvider.IsOwner)
			return Result.Failure<Guid>(sharedViewLocalizer.InvitationNoPermission(nameof(CancelInvitationCommand.Id)));

		var maybeItem = await dbContext.Invitations
			.FirstOrDefaultAsync(item =>
				item.SenderId == executionContextProvider.TenantId &&
				item.Status == Domain.Invitations.InvitationStatus.Sent, cancellationToken);

		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.InvitationNotFound(nameof(CancelInvitationCommand.Id)));

		dbContext.Invitations.Update(maybeItem.Cancel());

		await dbContext.SaveChangesAsync(cancellationToken);

		return Result.Success();
	}
}
