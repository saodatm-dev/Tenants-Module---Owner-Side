using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Identity.Application.Core.Abstractions.Data;

using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Invitations.Remove;

internal sealed class RemoveInvitationCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IExecutionContextProvider executionContextProvider,
	IIdentityDbContext dbContext) : ICommandHandler<RemoveInvitationCommand>
{
	public async Task<Result> Handle(RemoveInvitationCommand command, CancellationToken cancellationToken)
	{
		if (executionContextProvider.IsIndividual)
			return Result.Failure<Guid>(sharedViewLocalizer.InvitationNoPermission(nameof(RemoveInvitationCommand.Id)));

		if (!executionContextProvider.IsOwner)
			return Result.Failure<Guid>(sharedViewLocalizer.InvitationNoPermission(nameof(RemoveInvitationCommand.Id)));

		var maybeItem = await dbContext.Invitations
			.FirstOrDefaultAsync(item => item.SenderId == executionContextProvider.TenantId, cancellationToken);

		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.InvitationNotFound(nameof(RemoveInvitationCommand.Id)));

		dbContext.Invitations.Remove(maybeItem);

		await dbContext.SaveChangesAsync(cancellationToken);

		return Result.Success();
	}
}
