using Common.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Common.Application.Languages.Update;

internal sealed class UpdateLanguageCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	ICommonDbContext dbContext) : ICommandHandler<UpdateLanguageCommand, Guid>
{
	public async Task<Result<Guid>> Handle(UpdateLanguageCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.Languages.FirstOrDefaultAsync(item => item.Id == command.Id, cancellationToken);
		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.LanguageNotFound(nameof(UpdateLanguageCommand.Id)));

		dbContext.Languages.Update(maybeItem.Update(command.Name, command.ShortCode));

		await dbContext.SaveChangesAsync(cancellationToken);

		return maybeItem.Id;
	}
}
