using Common.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Common.Application.Languages.Remove;

internal sealed class RemoveLanguageCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	ICommonDbContext dbContext) : ICommandHandler<RemoveLanguageCommand>
{
	public async Task<Result> Handle(RemoveLanguageCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.Languages.FirstOrDefaultAsync(item => item.Id == command.Id, cancellationToken);
		if (maybeItem is null)
			return Result.Failure(sharedViewLocalizer.LanguageNotFound(nameof(RemoveLanguageCommand.Id)));

		dbContext.Languages.Remove(maybeItem);

		await dbContext.SaveChangesAsync(cancellationToken);

		return Result.Success();
	}
}
