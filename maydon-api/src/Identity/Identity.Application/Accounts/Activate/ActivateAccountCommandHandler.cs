using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Identity.Application.Core.Abstractions.Data;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Accounts.Activate;

internal sealed class ActivateAccountCommandHandler(
	IExecutionContextProvider executionContextProvider,
	ISharedViewLocalizer sharedViewLocalizer,
	IIdentityDbContext dbContext) : ICommandHandler<ActivateAccountCommand, Guid>
{
	public async Task<Result<Guid>> Handle(ActivateAccountCommand command, CancellationToken cancellationToken)
	{
		if (!executionContextProvider.IsOwner)
			return Result.Failure<Guid>(sharedViewLocalizer.InvitationNoPermission(nameof(ActivateAccountCommand.UserId)));

		var tenantId = executionContextProvider.TenantId;

		var account = await dbContext.Accounts
			.IgnoreQueryFilters([IApplicationDbContext.TenantIdFilter, IApplicationDbContext.IsActiveFilter])
			.FirstOrDefaultAsync(item => 
				item.TenantId == tenantId && 
				item.UserId == command.UserId &&
				!item.IsDeleted, 
				cancellationToken);

		if (account is null)
			return Result.Failure<Guid>(sharedViewLocalizer.UserNotFound(nameof(ActivateAccountCommand.UserId)));

		var companyUser = await dbContext.CompanyUsers
			.IgnoreQueryFilters([IApplicationDbContext.TenantIdFilter, IApplicationDbContext.IsActiveFilter])
			.FirstOrDefaultAsync(item => 
				item.CompanyId == tenantId && 
				item.UserId == command.UserId, 
				cancellationToken);

		account.Activate();
		dbContext.Accounts.Update(account);

		if (companyUser is not null)
		{
			companyUser.Activate();
			dbContext.CompanyUsers.Update(companyUser);
		}

		await dbContext.SaveChangesAsync(cancellationToken);

		return account.Id;
	}
}


