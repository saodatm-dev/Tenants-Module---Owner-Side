using Common.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Common.Application.Currencies.Remove;

internal sealed class RemoveCurrencyCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	ICommonDbContext dbContext) : ICommandHandler<RemoveCurrencyCommand>
{
	public async Task<Result> Handle(RemoveCurrencyCommand command, CancellationToken cancellationToken)
	{
		var maybeItem =
			await dbContext.Currencies.FirstOrDefaultAsync(item => item.Id == command.Id, cancellationToken);

		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.CurrencyNotFound(nameof(RemoveCurrencyCommand.Id)));

		dbContext.Currencies.Remove(maybeItem.Remove());

		await dbContext.SaveChangesAsync(cancellationToken);

		return Result.Success();
	}
}
