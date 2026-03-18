using Common.Application.Core.Abstractions.Data;
using Common.Domain.Languages;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Common.Application.Languages.Create;

internal sealed class CreateLanguageCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	ICommonDbContext dbContext) : ICommandHandler<CreateLanguageCommand, Guid>
{
	public async Task<Result<Guid>> Handle(CreateLanguageCommand command, CancellationToken cancellationToken)
	{
		if (await dbContext.Languages.AsNoTracking().AnyAsync(item => item.Name == command.Name.Trim() || item.ShortCode == command.ShortCode.Trim(), cancellationToken))
			return Result.Failure<Guid>(sharedViewLocalizer.AlreadyExists(nameof(CreateLanguageCommand.Name)));

		var language = new Language(
			command.Name,
			command.ShortCode);

		await dbContext.Languages.AddAsync(language, cancellationToken);

		await dbContext.SaveChangesAsync(cancellationToken);

		return language.Id;
	}
}
