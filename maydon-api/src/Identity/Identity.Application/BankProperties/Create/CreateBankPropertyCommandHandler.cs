using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Identity.Application.Core.Abstractions.Data;

using Identity.Domain.BankProperties;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.BankProperties.Create;

internal sealed class CreateBankPropertyCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IExecutionContextProvider executionContextProvider,
	IIdentityDbContext dbContext) : ICommandHandler<CreateBankPropertyCommand, Guid>
{
	public async Task<Result<Guid>> Handle(CreateBankPropertyCommand command, CancellationToken cancellationToken)
	{
		var tenantBankProperties = await dbContext.BankProperties.Where(item => item.TenantId == executionContextProvider.TenantId).ToListAsync(cancellationToken);
		if (tenantBankProperties.Any(item => item.AccountNumber == command.AccountNumber))
		{
			return Result.Failure<Guid>(sharedViewLocalizer.AlreadyExists(command.AccountNumber));
		}

		if (command.IsMain && tenantBankProperties.Any(item => item.IsMain))
		{
			// has main account number
			var isMainBankProperty = tenantBankProperties.Find(item => item.IsMain);
			if (isMainBankProperty is not null)
			{
				dbContext.BankProperties.Update(isMainBankProperty.DisableMain());
			}
		}

		var bankProperty = new BankProperty(
				executionContextProvider.TenantId,
				command.BankId,
				command.BankName,
				command.BankMfo,
				command.AccountNumber,
				command.IsMain,
				command.IsPublic);

		await dbContext.BankProperties.AddAsync(bankProperty, cancellationToken);

		await dbContext.SaveChangesAsync(cancellationToken);

		return bankProperty.Id;
	}
}
