using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Identity.Application.Core.Abstractions.Data;

using Microsoft.EntityFrameworkCore;

namespace Identity.Application.BankProperties.Remove;

internal sealed class RemoveBankAccountCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IIdentityDbContext dbContext) : ICommandHandler<RemoveBankAccountCommand>
{
	public async Task<Result> Handle(RemoveBankAccountCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.BankProperties.FirstOrDefaultAsync(item => item.Id == command.Id, cancellationToken);
		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.BankPropertyNotFound(nameof(RemoveBankAccountCommand.Id)));

		dbContext.BankProperties.Remove(maybeItem.Remove());

		await dbContext.SaveChangesAsync(cancellationToken);

		return Result.Success();
	}
}
