using Common.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Common.Application.Banks.Remove;

internal sealed class RemoveBankCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	ICommonDbContext dbContext) : ICommandHandler<RemoveBankCommand>
{
	public async Task<Result> Handle(RemoveBankCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.Banks.FirstOrDefaultAsync(item => item.Id == command.Id, cancellationToken);
		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.BankNotFound(nameof(RemoveBankCommand.Id)));

		dbContext.Banks.Remove(maybeItem.Remove());

		await dbContext.SaveChangesAsync(cancellationToken);

		return Result.Success();
	}
}
