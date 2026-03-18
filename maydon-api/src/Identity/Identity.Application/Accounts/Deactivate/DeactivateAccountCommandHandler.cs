using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Identity.Application.Core.Abstractions.Data;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Accounts.Deactivate;

internal sealed class DeactivateAccountCommandHandler(
	IExecutionContextProvider executionContextProvider,
	ISharedViewLocalizer sharedViewLocalizer,
	IIdentityDbContext dbContext) : ICommandHandler<DeactivateAccountCommand, Guid>
{
	public async Task<Result<Guid>> Handle(DeactivateAccountCommand command, CancellationToken cancellationToken)
	{
		if (!executionContextProvider.IsOwner)
			return Result.Failure<Guid>(sharedViewLocalizer.InvitationNoPermission(nameof(DeactivateAccountCommand.UserId)));

		if (command.UserId == executionContextProvider.UserId)
			return Result.Failure<Guid>(sharedViewLocalizer.CannotDeactivateOwnAccount(nameof(DeactivateAccountCommand.UserId)));

		var tenantId = executionContextProvider.TenantId;

		var account = await dbContext.Accounts
			.IgnoreQueryFilters([IApplicationDbContext.TenantIdFilter, IApplicationDbContext.IsActiveFilter])
			.FirstOrDefaultAsync(item => 
				item.TenantId == tenantId && 
				item.UserId == command.UserId, 
				cancellationToken);

		if (account is null)
			return Result.Failure<Guid>(sharedViewLocalizer.UserNotFound(nameof(DeactivateAccountCommand.UserId)));

		var companyUser = await dbContext.CompanyUsers
			.IgnoreQueryFilters([IApplicationDbContext.TenantIdFilter])
			.FirstOrDefaultAsync(item => 
				item.CompanyId == tenantId && 
				item.UserId == command.UserId, 
				cancellationToken);

		account.Deactivate();
		dbContext.Accounts.Update(account);

		if (companyUser is not null)
		{
			companyUser.Deactivate();
			dbContext.CompanyUsers.Update(companyUser);
		}

		await dbContext.SaveChangesAsync(cancellationToken);

		return account.Id;
	}
}

