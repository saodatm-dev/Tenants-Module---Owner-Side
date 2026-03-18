using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Providers;
using Core.Domain.Results;
using Identity.Application.Core.Abstractions.Cryptors;
using Identity.Application.Core.Abstractions.Data;

using Identity.Domain.Invitations;
using Microsoft.EntityFrameworkCore;


namespace Identity.Application.Invitations.Create;

internal sealed class CreateInvitationCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IExecutionContextProvider executionContextProvider,
	IDateTimeProvider dateTimeProvider,
	IIdentityDbContext dbContext,
	ICryptor cryptor) : ICommandHandler<CreateInvitationCommand, Guid>
{
	private readonly DateTime expiredTime = dateTimeProvider.UtcNow.AddDays(2);
	public async Task<Result<Guid>> Handle(CreateInvitationCommand command, CancellationToken cancellationToken)
	{
		if (executionContextProvider.IsIndividual)
			return Result.Failure<Guid>(sharedViewLocalizer.IndividualUserCanNotInvite(nameof(CreateInvitationCommand.PhoneNumber)));

		if (!executionContextProvider.IsOwner)
			return Result.Failure<Guid>(sharedViewLocalizer.InvitationNoPermission(nameof(CreateInvitationCommand.PhoneNumber)));

		var maybeRoleId = await dbContext.Roles
			.AsNoTracking()
			.Where(item => item.Id == command.RoleId)
			.Select(item => item.Id)
			.FirstOrDefaultAsync(cancellationToken);

		if (maybeRoleId == Guid.Empty)
			return Result.Failure<Guid>(sharedViewLocalizer.RoleNotFound(nameof(CreateInvitationCommand.RoleId)));

		var normalizedPhone = command.PhoneNumber.Trim().TrimStart('+');

		var recipient = await dbContext.Users
			.AsNoTracking()
			.Where(item => item.PhoneNumber == normalizedPhone || item.PhoneNumber == command.PhoneNumber.Trim())
			.Select(item => new { item.Id, item.FullName })
			.FirstOrDefaultAsync(cancellationToken);

		Invitation invitation;
		if (recipient is not null)
		{
			invitation = new Invitation(
				executionContextProvider.TenantId,
				recipient.Id,
				maybeRoleId,
				sharedViewLocalizer.InvitationContentByUserIdContent(recipient.FullName.Trim()),
				expiredTime);
		}
		else
		{
			var companyName = await dbContext.Companies
				.AsNoTracking()
				.Where(item => item.Id == executionContextProvider.TenantId)
				.Select(item => item.Name)
				.FirstOrDefaultAsync(cancellationToken) ?? "Maydon";

			invitation = new Invitation(
				executionContextProvider.TenantId,
				command.PhoneNumber.Trim(),
				maybeRoleId,
				sharedViewLocalizer.InvitationContentByPhoneNumber(companyName),
				expiredTime);
		}

		invitation.Key = cryptor.EncryptInvitation(invitation.Id);

		await dbContext.Invitations.AddAsync(invitation, cancellationToken);

		await dbContext.SaveChangesAsync(cancellationToken);

		return invitation.Id;
	}
}
