using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Identity.Application.Core.Abstractions.Data;

using Microsoft.EntityFrameworkCore;

namespace Identity.Application.BankProperties.Update;

internal sealed class UpdateBankPropertyCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IExecutionContextProvider executionContextProvider,
	IIdentityDbContext dbContext) : ICommandHandler<UpdateBankPropertyCommand, Guid>
{
	public async Task<Result<Guid>> Handle(UpdateBankPropertyCommand command, CancellationToken cancellationToken)
	{
		var tenantBankProperties = await dbContext.BankProperties.Where(item => item.TenantId == executionContextProvider.TenantId).ToListAsync(cancellationToken);

		var maybeItem = tenantBankProperties.Find(item => item.Id == command.Id);
		if (maybeItem is null)
		{
			return Result.Failure<Guid>(sharedViewLocalizer.BankPropertyNotFound(nameof(UpdateBankPropertyCommand.Id)));
		}

		if (tenantBankProperties.Any(item => item.AccountNumber == command.AccountNumber && item.Id != command.Id))
		{
			return Result.Failure<Guid>(sharedViewLocalizer.BankPropertyAlreadyExists(command.AccountNumber));
		}

		if (command.IsMain && tenantBankProperties.Any(item => item.IsMain && item.Id != command.Id))
		{
			// has main account number
			var isMainBankProperty = tenantBankProperties.Find(item => item.IsMain);
			if (isMainBankProperty is not null)
			{
				dbContext.BankProperties.Update(isMainBankProperty.DisableMain());
			}
		}

		dbContext.BankProperties.Update(
			maybeItem.Update(
				command.BankId,
				command.BankName,
				command.BankMfo,
				command.AccountNumber,
				command.IsMain,
				command.IsPublic));

		await dbContext.SaveChangesAsync(cancellationToken);

		return maybeItem.Id;
	}
}
