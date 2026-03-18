using Common.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Common.Application.Banks.Update;

internal sealed class UpdateBankCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	ICommonDbContext dbContext) : ICommandHandler<UpdateBankCommand, Guid>
{
	public async Task<Result<Guid>> Handle(UpdateBankCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.Banks.FirstOrDefaultAsync(item => item.Id == command.Id, cancellationToken);
		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.BankNotFound(nameof(UpdateBankCommand.Id)));

		var isExist = await dbContext.Banks
			.AsNoTracking()
			.AnyAsync(item =>
				item.Mfo == command.Mfo &&
				(!string.IsNullOrWhiteSpace(command.Tin) ? item.Tin == command.Tin : true), cancellationToken);

		if (!isExist)
			return Result.Failure<Guid>(sharedViewLocalizer.BankAlreadyExists(nameof(UpdateBankCommand.Mfo)));

		var languages = await dbContext.Languages.AsNoTracking().ToListAsync(cancellationToken);

		for (int i = 0; i < command.Translates.Count; i++)
		{
			var languageValue = command.Translates[i];
			var language = languages.Find(item => item.Id == languageValue.LanguageId);
			if (language is null)
				return Result.Failure<Guid>(sharedViewLocalizer.LanguageNotFound($"{nameof(UpdateBankCommand.Translates)}.{languageValue.LanguageId}"));

			command.Translates[i] = languageValue with { LanguageShortCode = language.ShortCode };
		}

		dbContext.Banks.Update(
			maybeItem.Update(
				command.Mfo,
				command.Tin,
				command.PhoneNumber,
				command.Email,
				command.Website,
				command.Address,
				command.Translates));

		await dbContext.SaveChangesAsync(cancellationToken);

		return maybeItem.Id;
	}
}
