using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Providers;
using Core.Domain.Results;
using Identity.Application.Core.Abstractions.Data;

using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Invitations.Accept;

internal sealed class AcceptInvitationCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IDateTimeProvider dateTimeProvider,
	IExecutionContextProvider executionContextProvider,
	IIdentityDbContext dbContext) : ICommandHandler<AcceptInvitationCommand>
{
	public async Task<Result> Handle(AcceptInvitationCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.Invitations
			.FirstOrDefaultAsync(item =>
				item.Id == command.Id &&
				item.RecipientId == executionContextProvider.UserId &&
				item.Status == Domain.Invitations.InvitationStatus.Sent, cancellationToken);

		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.InvitationNotFound(nameof(AcceptInvitationCommand.Id)));

		if (maybeItem.IsExpired(dateTimeProvider.UtcNow))
			return Result.Failure<Guid>(sharedViewLocalizer.InvitationExpired(nameof(AcceptInvitationCommand.Id)));

		dbContext.Invitations.Update(maybeItem.Accept());

		await dbContext.SaveChangesAsync(cancellationToken);

		return Result.Success();
	}
}
